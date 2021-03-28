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
                    ImageUrl = game.Cover.Value.Url,
                    ImageHeight = game.Cover.Value.Height.GetValueOrDefault(),
                    ImageWidth = game.Cover.Value.Width.GetValueOrDefault(),
                    GamingPlatforms = game.Platforms.Values.Select(platform => new GamingPlatformDto()
                    {
                        ApiId = platform.Id.Value,
                        Abbreviation = platform.Abbreviation,
                        Name = platform.Name,
                        LogoURL = platform.Url
                    }).ToList(),
                    GameGenres = game.Genres.Values.Select(genre => new GameGenreDto()
                    {
                        ApiId = genre.Id.Value,
                        Name = genre.Name,
                        Slug = genre.Slug
                    }).ToList()
                });

                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error searching for games!");
            }
        }

        [HttpGet("userGames")]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetUserGames()
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error retreiving games for the user");
                }

                var idUser = GetUserId();
                var userGames = await _context.ApplicationUser_Games.Where(mapping => mapping.ApplicationUserId == idUser)
                    .Include(mapping => mapping.Game)
                    .Include(mapping => mapping.Game.GameCover).ToListAsync();

                var gameDtos = userGames.Select(mapping => mapping.Game).Select(game => new GameDto()
                {
                    Id = game.Id,
                    ApiId = game.ApiId,
                    Name = game.Name,
                    Summary = game.Summary,
                    ReleaseDate = game.ReleaseDate,
                    ImageUrl = game.GameCover.ImageUrl,
                    ImageHeight = game.GameCover.Height,
                    ImageWidth = game.GameCover.Width
                });

                return Ok(gameDtos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retreiving games for the user");
            }
        }

        [HttpPost("addUserGame")]
        public async Task<IActionResult> AddUserGame(GameDto gameDto)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error adding game to the user");
                }

                var idUser = GetUserId();

                if (!gameDto.Id.HasValue)
                {
                    if (await _context.Games.AnyAsync(game => game.ApiId == gameDto.ApiId))
                    {
                        gameDto.Id = (await _context.Games.FirstOrDefaultAsync(game => game.ApiId == gameDto.ApiId)).Id;
                    }
                    else
                    {
                        gameDto.Id = await CreateGame(gameDto);
                    }
                }

                _context.ApplicationUser_Games.Add(new ApplicationUser_Game()
                {
                    GameId = gameDto.Id.Value,
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

        private async Task<int> CreateGame(GameDto gameDto)
        {
            var newApiGame = await _videoGameRepository.GetGameAsync(gameDto.ApiId);

            var newGame = new Game()
            {
                ApiId = newApiGame.Id.Value,
                Name = newApiGame.Name,
                Summary = newApiGame.Summary,
                ReleaseDate = newApiGame.FirstReleaseDate.GetValueOrDefault().DateTime
            };

            await _context.Games.AddAsync(newGame);
            await _context.SaveChangesAsync();

            newGame = await _context.Games.FirstOrDefaultAsync(game => game.ApiId == newGame.ApiId);

            var newGameCover = new GameCover()
            {
                GameId = newGame.Id,
                ImageUrl = newApiGame.Cover.Value.Url,
                Height = newApiGame.Cover.Value.Height.GetValueOrDefault(),
                Width = newApiGame.Cover.Value.Width.GetValueOrDefault()
            };

            await _context.GameCovers.AddAsync(newGameCover);
            await _context.SaveChangesAsync();

            var gamingPlatforms = await _context.GamingPlatforms.ToListAsync();

            var platformIds = from platform in gamingPlatforms
                              join apiPlatform in newApiGame.Platforms.Values on platform.ApiId equals apiPlatform.Id into existingPlatforms
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
                               join apiGenre in newApiGame.Genres.Values on genre.ApiId equals apiGenre.Id into existingGenres
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

        [HttpDelete("deleteUserGame/{id}")]
        public async Task<IActionResult> RemoveUserGame(int id)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error removing game from the user");
                }

                var idUser = GetUserId();

                var userGame = await _context.ApplicationUser_Games.Where(mapping => mapping.ApplicationUserId == idUser && mapping.GameId == id).FirstOrDefaultAsync();
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
