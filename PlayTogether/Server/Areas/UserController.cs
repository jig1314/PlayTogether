using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlayTogether.Server.Data;
using PlayTogether.Server.Models;
using PlayTogether.Shared.Models;
using PlayTogether.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Server.Areas
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet("genders")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Gender>>> GetAllGenders()
        {
            try
            {
                var genders = await _context.Genders.ToListAsync();
                return Ok(genders);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving genders");
            }
        }

        [HttpGet("countries")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Gender>>> GetAllCountries()
        {
            try
            {
                var countries = await _context.Countries.ToListAsync();
                return Ok(countries);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving countries");
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> RegisterNewUser(RegisterUserDto registerUserDto)
        {
            try
            {
                var user = new ApplicationUser { UserName = registerUserDto.UserName, Email = registerUserDto.Email };
                var result = await _userManager.CreateAsync(user, registerUserDto.Password);
                var userDetails = new ApplicationUserDetails()
                {
                    ApplicationUserId = user.Id,
                    FirstName = registerUserDto.FirstName,
                    LastName = registerUserDto.LastName,
                    DateOfBirth = registerUserDto.DateOfBirth,
                    CountryOfResidenceId = registerUserDto.CountryOfResidenceId,
                    GenderId = registerUserDto.GenderId
                };
                _context.ApplicationUserDetails.Add(userDetails);
                await _context.SaveChangesAsync();

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return StatusCode(StatusCodes.Status201Created);
                }

                throw new Exception("User registration failed!");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
