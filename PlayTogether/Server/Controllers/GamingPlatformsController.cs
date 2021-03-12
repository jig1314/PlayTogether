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
using PlayTogether.Shared.DTOs;

namespace PlayTogether.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamingPlatformsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GamingPlatformsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetUserId()
        {
            return HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet("userGamingPlatforms")]
        public async Task<ActionResult<IEnumerable<GamingPlatformDto>>> GetGamingPlatforms()
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error retreiving gaming platforms for the user");
                }
                
                var idUser = GetUserId();
                var userGamingPlatforms = await _context.UserGamingPlatforms.Where(mapping => mapping.ApplicationUserId == idUser).Include(mapping => mapping.GamingPlatform).ToListAsync();
                var gamingPlatformDtos = userGamingPlatforms.Select(mapping => mapping.GamingPlatform).Select(platform => new GamingPlatformDto()
                {
                    Id = platform.Id,
                    Abbreviation = platform.Abbreviation,
                    Name = platform.Name,
                    LogoURL = platform.LogoURL
                });

                return Ok(gamingPlatformDtos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retreiving gaming platforms for the user");
            }
        }

        [HttpPut("updateUserGamingPlatforms")]
        public async Task<IActionResult> PutGamingPlatforms(List<GamingPlatformDto> gamingPlatforms)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error updating gaming platforms for the user");
                }

                var idUser = GetUserId();
                var userGamingPlatforms = await _context.UserGamingPlatforms.Where(mapping => mapping.ApplicationUserId == idUser).Include(mapping => mapping.GamingPlatform).ToListAsync();

                var userGamingPlatformsToDelete = userGamingPlatforms.Where(mapping => !gamingPlatforms.Select(platform => platform.Id).Contains(mapping.GamingPlatformId));
                var userGamingPlatformsToInsert = gamingPlatforms.Where(platform => !userGamingPlatforms.Select(mapping => mapping.GamingPlatformId).Contains(platform.Id));

                _context.UserGamingPlatforms.RemoveRange(userGamingPlatformsToDelete);
                _context.UserGamingPlatforms.AddRange(userGamingPlatformsToInsert.Select(platform => new ApplicationUser_GamingPlatform()
                {
                    GamingPlatformId = platform.Id,
                    ApplicationUserId = idUser
                }));

                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status202Accepted);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating gaming platforms for the user");
            }
        }
    }
}
