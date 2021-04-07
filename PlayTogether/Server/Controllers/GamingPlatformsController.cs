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

        [HttpGet("gamingPlatforms")]
        public async Task<ActionResult<IEnumerable<GamingPlatformDto>>> GetGamingPlatforms()
        {
            try
            {
                var gamingPlatforms = await _context.GamingPlatforms.ToListAsync();
                var gamingPlatformDtos = gamingPlatforms.Select(platform => new GamingPlatformDto()
                {
                    Id = platform.Id,
                    ApiId = platform.ApiId,
                    Abbreviation = platform.Abbreviation,
                    Name = platform.Name,
                    LogoURL = platform.LogoURL
                });

                return Ok(gamingPlatformDtos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retreiving gaming platforms");
            }
        }

        [HttpGet("userGamingPlatforms")]
        public async Task<ActionResult<IEnumerable<GamingPlatformDto>>> GetUserGamingPlatforms()
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error retreiving gaming platforms for the user");
                }
                
                var idUser = GetUserId();
                var userGamingPlatforms = await _context.ApplicationUser_GamingPlatform.Where(mapping => mapping.ApplicationUserId == idUser).Include(mapping => mapping.GamingPlatform).ToListAsync();
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

        [HttpPost("addUserGamingPlatform")]
        public async Task<IActionResult> AddUserGamingPlatform(GamingPlatformDto gamingPlatform)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error updating gaming platforms for the user");
                }

                var idUser = GetUserId();

                _context.ApplicationUser_GamingPlatform.Add(new ApplicationUser_GamingPlatform()
                {
                    GamingPlatformId = gamingPlatform.Id,
                    ApplicationUserId = idUser
                });

                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status202Accepted);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating gaming platforms for the user");
            }
        }

        [HttpDelete("deleteUserGamingPlatform/{id}")]
        public async Task<IActionResult> RemoveUserGamingPlatform(int id)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error updating gaming platforms for the user");
                }

                var idUser = GetUserId();

                var userGamingPlatform = await _context.ApplicationUser_GamingPlatform.Where(mapping => mapping.ApplicationUserId == idUser && mapping.GamingPlatformId == id).FirstOrDefaultAsync();
                _context.ApplicationUser_GamingPlatform.Remove(userGamingPlatform);

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
