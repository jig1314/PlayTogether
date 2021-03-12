using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlayTogether.Server.Data;
using PlayTogether.Server.Models;
using PlayTogether.Shared.DTOs;
using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PlayTogether.Server.Controllers
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

        private string GetUserId()
        {
            return HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet("accountInfo")]
        public async Task<ActionResult<UserAccountDto>> GetAccountInformation()
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error retrieving user account information");
                }

                var idUser = GetUserId();
                var user = await _context.Users.Where(d => d.Id == idUser).Include(user => user.ApplicationUserDetails).FirstOrDefaultAsync();
                var userAccountDto = new UserAccountDto()
                {
                    UserName = user.UserName,
                    FirstName = user.ApplicationUserDetails.FirstName,
                    LastName = user.ApplicationUserDetails.LastName,
                    Email = user.Email,
                    DateOfBirth = user.ApplicationUserDetails.DateOfBirth,
                    CountryOfResidenceId = user.ApplicationUserDetails.CountryOfResidenceId,
                    GenderId = user.ApplicationUserDetails.GenderId,
                    PhoneNumber = user.PhoneNumber
                };

                return Ok(userAccountDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving user account information");
            }
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
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmEmailResult = await _userManager.ConfirmEmailAsync(user, code);
                    
                    if (confirmEmailResult.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status201Created);
                    }

                    throw new Exception("User registration succeeded but failed to confirm the email!");
                }

                throw new Exception("User registration failed!");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginDto login)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, login.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status202Accepted);
                }

                throw new Exception("Login failed!");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("resetPassword")]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(resetPasswordDto.UserName);
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, code, resetPasswordDto.Password);

                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status202Accepted);
                }

                throw new Exception("Password Reset failed!");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("updateAccountInfo")]
        public async Task<ActionResult<UserAccountDto>> UpdateUserAccountInformation(UserAccountDto userAccountDto)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error updating user account information");
                }

                var idUser = GetUserId();
                var user = await _userManager.FindByIdAsync(idUser);

                if (user.Email != userAccountDto.Email)
                {
                    var emailToken = await _userManager.GenerateChangeEmailTokenAsync(user, userAccountDto.Email);
                    await _userManager.ChangeEmailAsync(user, userAccountDto.Email, emailToken);
                }

                if (user.PhoneNumber != userAccountDto.PhoneNumber)
                {
                    var phoneNumberToken = await _userManager.GenerateChangePhoneNumberTokenAsync(user, userAccountDto.PhoneNumber);
                    await _userManager.ChangePhoneNumberAsync(user, userAccountDto.PhoneNumber, phoneNumberToken);
                }

                var applicationUserDetails = await _context.ApplicationUserDetails.FirstOrDefaultAsync(detail => detail.ApplicationUserId == idUser);
                applicationUserDetails.FirstName = userAccountDto.FirstName;
                applicationUserDetails.LastName = userAccountDto.LastName;
                applicationUserDetails.DateOfBirth = userAccountDto.DateOfBirth;
                applicationUserDetails.CountryOfResidenceId = userAccountDto.CountryOfResidenceId;
                applicationUserDetails.GenderId = userAccountDto.GenderId;

                _context.Entry(applicationUserDetails).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status202Accepted);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving user account information");
            }
        }

        [HttpPut("changePassword")]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error updating user account information");
                }

                var idUser = GetUserId();
                var user = await _userManager.FindByIdAsync(idUser);
                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);

                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status202Accepted);
                }

                throw new Exception("Password Reset failed!");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
