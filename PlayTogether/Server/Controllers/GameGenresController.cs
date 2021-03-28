using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayTogether.Server.Data;
using PlayTogether.Server.Models;
using PlayTogether.Server.Repositories;
using PlayTogether.Shared.DTOs;

namespace PlayTogether.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameGenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IVideoGameRepository _videoGameRepository;

        public GameGenresController(ApplicationDbContext context, IVideoGameRepository videoGameRepository)
        {
            _context = context;
            _videoGameRepository = videoGameRepository;
        }

        private string GetUserId()
        {
            return HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet("gameGenres")]
        public async Task<ActionResult<IEnumerable<GameGenreDto>>> GetGameGenres()
        {
            try
            {
                var gameGenres = await _context.GameGenres.ToListAsync();
                var gameGenreDtos = gameGenres.Select(genre => new GameGenreDto()
                {
                    Id = genre.Id,
                    Name = genre.Name,
                    Slug = genre.Slug
                });

                return Ok(gameGenreDtos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retreiving game genres!");
            }
        }

        [HttpGet("userGameGenres")]
        public async Task<ActionResult<IEnumerable<GameGenreDto>>> GetUserGameGenres()
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error retreiving game genres for the user");
                }

                var idUser = GetUserId();
                var userGameGenres = await _context.ApplicationUser_GameGenres.Where(mapping => mapping.ApplicationUserId == idUser).Include(mapping => mapping.GameGenre).ToListAsync();

                var gameGenreDtos = userGameGenres.Select(mapping => mapping.GameGenre).Select(genre => new GameGenreDto()
                {
                    Id = genre.Id,
                    Name = genre.Name,
                    Slug = genre.Slug
                });

                return Ok(gameGenreDtos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retreiving gaming genres for the user");
            }
        }

        [HttpPut("updateGameGenres")]
        public async Task<IActionResult> PutUserGameGenres(List<int> gameGenreIds)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error updating game genres for the user");
                }

                var idUser = GetUserId();
                var userGameGenres = await _context.ApplicationUser_GameGenres.Where(mapping => mapping.ApplicationUserId == idUser).Include(mapping => mapping.GameGenre).ToListAsync();

                var userGameGenresToDelete = userGameGenres.Where(mapping => !gameGenreIds.Contains(mapping.GameGenreId));
                var userGameGenresToInsert = gameGenreIds.Where(id => !userGameGenres.Select(mapping => mapping.GameGenreId).Contains(id));

                _context.ApplicationUser_GameGenres.RemoveRange(userGameGenresToDelete);
                _context.ApplicationUser_GameGenres.AddRange(userGameGenresToInsert.Select(id => new ApplicationUser_GameGenre()
                {
                    GameGenreId = id,
                    ApplicationUserId = idUser
                }));

                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status202Accepted);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating gaming genres for the user");
            }
        }

        // GET: api/GameGenres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameGenre>> GetGameGenre(int id)
        {
            var gameGenre = await _context.GameGenres.FindAsync(id);

            if (gameGenre == null)
            {
                return NotFound();
            }

            return gameGenre;
        }

        // PUT: api/GameGenres/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGameGenre(int id, GameGenre gameGenre)
        {
            if (id != gameGenre.Id)
            {
                return BadRequest();
            }

            _context.Entry(gameGenre).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameGenreExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/GameGenres
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<GameGenre>> PostGameGenre(GameGenre gameGenre)
        {
            _context.GameGenres.Add(gameGenre);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGameGenre", new { id = gameGenre.Id }, gameGenre);
        }

        // DELETE: api/GameGenres/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<GameGenre>> DeleteGameGenre(int id)
        {
            var gameGenre = await _context.GameGenres.FindAsync(id);
            if (gameGenre == null)
            {
                return NotFound();
            }

            _context.GameGenres.Remove(gameGenre);
            await _context.SaveChangesAsync();

            return gameGenre;
        }

        private bool GameGenreExists(int id)
        {
            return _context.GameGenres.Any(e => e.Id == id);
        }
    }
}
