using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
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
using System.Text;
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
                var password = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(registerUserDto.Password));

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
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

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmEmailResult = await _userManager.ConfirmEmailAsync(user, code);
                    
                    if (confirmEmailResult.Succeeded)
                        return StatusCode(StatusCodes.Status201Created);

                    throw new Exception(string.Join(System.Environment.NewLine, confirmEmailResult.Errors.Select(error => error.Description)));
                }

                throw new Exception(string.Join(System.Environment.NewLine, result.Errors.Select(error => error.Description)));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, (ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginDto login)
        {
            try
            {
                var password = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(login.Password));
                var result = await _signInManager.PasswordSignInAsync(login.UserName, password, login.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                    return StatusCode(StatusCodes.Status202Accepted);

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

                if (user == null)
                    throw new Exception("Username could not be found!");

                var password = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPasswordDto.Password));
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                var result = await _userManager.ResetPasswordAsync(user, code, password);

                if (result.Succeeded)
                    return StatusCode(StatusCodes.Status202Accepted);

                throw new Exception(string.Join(System.Environment.NewLine, result.Errors.Select(error => error.Description)));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, (ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
            }
        }

        [HttpPut("updateAccountInfo")]
        public async Task<ActionResult<UserAccountDto>> UpdateUserAccountInformation(UserAccountDto userAccountDto)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "You are not authorized to update account information!");
                }

                var idUser = GetUserId();
                var user = await _userManager.FindByIdAsync(idUser);

                if (user.Email != userAccountDto.Email)
                {
                    var emailToken = await _userManager.GenerateChangeEmailTokenAsync(user, userAccountDto.Email);
                    var changeEmailResult = await _userManager.ChangeEmailAsync(user, userAccountDto.Email, emailToken);

                    if (!changeEmailResult.Succeeded)
                        throw new Exception(string.Join(System.Environment.NewLine, changeEmailResult.Errors.Select(error => error.Description)));
                }

                if (user.PhoneNumber != userAccountDto.PhoneNumber)
                {
                    var phoneNumberToken = await _userManager.GenerateChangePhoneNumberTokenAsync(user, userAccountDto.PhoneNumber);
                    var changePhoneNumberResult = await _userManager.ChangePhoneNumberAsync(user, userAccountDto.PhoneNumber, phoneNumberToken);

                    if (!changePhoneNumberResult.Succeeded)
                        throw new Exception(string.Join(System.Environment.NewLine, changePhoneNumberResult.Errors.Select(error => error.Description)));
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
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, (ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
            }
        }

        [HttpPut("changePassword")]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "You are not authorized to change a password!");
                }

                var idUser = GetUserId();
                var user = await _userManager.FindByIdAsync(idUser);

                var oldPassword = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(changePasswordDto.OldPassword));
                var newPassword = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(changePasswordDto.NewPassword));

                var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

                if (result.Succeeded)
                    return StatusCode(StatusCodes.Status202Accepted);

                throw new Exception(string.Join(System.Environment.NewLine, result.Errors.Select(error => error.Description)));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, (ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
            }
        }

        [HttpDelete("delete/{userName}")]
        [AllowAnonymous]
        public async Task<ActionResult> DeleteUser(string userName)
        {
            try
            {
                var user = await _context.Users.Include(user => user.ApplicationUserDetails).FirstOrDefaultAsync(user => user.UserName == userName);

                _context.ApplicationUserDetails.Remove(user.ApplicationUserDetails);
                await _context.SaveChangesAsync();

                await _userManager.DeleteAsync(user);

                return StatusCode(StatusCodes.Status202Accepted);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, (ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}
