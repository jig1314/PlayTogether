using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayTogether.Server.Data;
using PlayTogether.Server.Models;
using PlayTogether.Server.Repositories;
using PlayTogether.Shared.DTOs;
using PlayTogether.Shared.Models;

namespace PlayTogether.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IVideoGameRepository _videoGameRepository;

        public GamesController(ApplicationDbContext context, IVideoGameRepository videoGameRepository)
        {
            _context = context;
            _videoGameRepository = videoGameRepository;
        }

        private string GetUserId()
        {
            return HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpPut("search")]
        public async Task<ActionResult<IEnumerable<GameSearchResult>>> SearchGames(GameSearchDto gameSearch)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error searching for games!");
                }

                var games = await _videoGameRepository.GetGamesAsync(gameSearch);

                var results = games.Select(game => new GameSearchResult()
                {
                    ApiId = game.Id.Value,
                    Name = game.Name,
                    Summary = game.Summary,
                    ReleaseDate = game.FirstReleaseDate.GetValueOrDefault().DateTime,
                    ImageUrl = game.Cover?.Value?.Url
                });

                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error searching for games!");
            }
        }

        [HttpGet("userGames")]
        public async Task<ActionResult<IEnumerable<UserGameDto>>> GetUserGames()
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error retreiving games for the user");
                }

                var idUser = GetUserId();
                var userGames = await _context.ApplicationUser_Games.Where(mapping => mapping.ApplicationUserId == idUser)
                    .Include(mapping => mapping.Game).ToListAsync();

                var gameDtos = userGames.Select(mapping => mapping).Select(mapping => new UserGameDto()
                {
                    Id = mapping.Game.Id,
                    ApiId = mapping.Game.ApiId,
                    Name = mapping.Game.Name,
                    Summary = mapping.Game.Summary,
                    ReleaseDate = mapping.Game.ReleaseDate,
                    ImageUrl = mapping.Game.ImageUrl,
                    GameSkillLevelId = mapping.GameSkillLevelId
                });

                return Ok(gameDtos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retreiving games for the user");
            }
        }

        [HttpGet("gameSkillLevels")]
        public async Task<ActionResult<IEnumerable<GameSkillLevel>>> GetGameSkillLevels()
        {
            try
            {
                var gameSkillLevels = await _context.GameSkillLevels.ToListAsync();

                return Ok(gameSkillLevels);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retreiving game skill levels!");
            }
        }

        [HttpPost("addUserGame")]
        public async Task<IActionResult> AddUserGame(UserGameDto game)
        {
            try
            {
                var apiId = game.ApiId;

                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error adding game to the user");
                }

                var idUser = GetUserId();

                int newGameId;

                if (await _context.Games.AnyAsync(game => game.ApiId == apiId))
                {
                    newGameId = (await _context.Games.FirstOrDefaultAsync(game => game.ApiId == apiId)).Id;
                }
                else
                {
                    newGameId = await CreateGame(apiId);
                }

                _context.ApplicationUser_Games.Add(new ApplicationUser_Game()
                {
                    GameId = newGameId,
                    ApplicationUserId = idUser
                });

                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status202Accepted);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error adding game to the user");
            }
        }

        private async Task<int> CreateGame(long apiId)
        {
            var newApiGame = await _videoGameRepository.GetGameAsync(apiId);

            var newGame = new Game()
            {
                ApiId = newApiGame.Id.Value,
                Name = newApiGame.Name,
                Summary = newApiGame.Summary,
                ReleaseDate = newApiGame.FirstReleaseDate.GetValueOrDefault().DateTime,
                ImageUrl = newApiGame.Cover?.Value?.Url
            };

            await _context.Games.AddAsync(newGame);
            await _context.SaveChangesAsync();

            newGame = await _context.Games.FirstOrDefaultAsync(game => game.ApiId == newGame.ApiId);

            var gamingPlatforms = await _context.GamingPlatforms.ToListAsync();

            var platformIds = from platform in gamingPlatforms
                              join apiPlatformId in newApiGame.Platforms?.Ids on platform.ApiId equals apiPlatformId into existingPlatforms
                              from existingPlatform in existingPlatforms
                              select platform.Id;

            _context.GamingPlatform_Games.AddRange(platformIds.Select(id => new GamingPlatform_Game()
            {
                GamingPlatformId = id,
                GameId = newGame.Id
            }));

            await _context.SaveChangesAsync();

            var gameGenres = await _context.GameGenres.ToListAsync();

            var gameGenreIds = from genre in gameGenres
                               join apiGenreId in newApiGame.Genres?.Ids on genre.ApiId equals apiGenreId into existingGenres
                               from existingGenre in existingGenres
                               select genre.Id;

            _context.GameGenre_Games.AddRange(gameGenreIds.Select(id => new GameGenre_Game()
            {
                GameGenreId = id,
                GameId = newGame.Id
            }));

            await _context.SaveChangesAsync();

            return newGame.Id;
        }

        [HttpPut("updateUserGameSkillLevel")]
        public async Task<ActionResult> UpdateUserGameSkillLevel(UserGameDto userGameDto)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error update game skill level for the user");
                }

                var idUser = GetUserId();
                var userGame = await _context.ApplicationUser_Games.FirstOrDefaultAsync(x => x.ApplicationUserId == idUser && x.GameId == userGameDto.Id.Value);
                userGame.GameSkillLevelId = userGameDto.GameSkillLevelId;

                _context.Entry(userGame).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status202Accepted);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error update game skill level for the user");
            }
        }

        [HttpDelete("deleteUserGame/{apiId}")]
        public async Task<IActionResult> RemoveUserGame(long apiId)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error removing game from the user");
                }

                var idUser = GetUserId();

                var game = await _context.Games.FirstOrDefaultAsync(game => game.ApiId == apiId);
                var userGame = await _context.ApplicationUser_Games.Where(mapping => mapping.ApplicationUserId == idUser && mapping.GameId == game.Id).FirstOrDefaultAsync();
                _context.ApplicationUser_Games.Remove(userGame);

                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status202Accepted);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error removing game from the user");
            }
        }
    }
}
