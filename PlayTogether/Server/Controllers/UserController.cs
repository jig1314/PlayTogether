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

        [HttpGet("profile/{userName}")]
        public async Task<ActionResult<UserProfileDto>> GetUserProfileInformation(string userName)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error retrieving user profile information");
                }

                var idUser = (await _context.Users.Where(d => d.UserName == userName).FirstOrDefaultAsync()).Id;

                var user = await _context.Users.Where(d => d.UserName == userName)
                    .Include(user => user.ApplicationUserDetails)
                    .Include(user => user.ApplicationUserDetails.CountryOfResidence)
                    .Include(user => user.ApplicationUserDetails.Gender)
                    .FirstOrDefaultAsync();

                var gamingPlatforms = await _context.ApplicationUser_GamingPlatform.Where(mapping => mapping.ApplicationUserId == idUser)
                    .Include(mapping => mapping.GamingPlatform)
                    .ToListAsync();

                var gameGenres = await _context.ApplicationUser_GameGenres.Where(mapping => mapping.ApplicationUserId == idUser)
                    .Include(mapping => mapping.GameGenre)
                    .ToListAsync();

                var games = await _context.ApplicationUser_Games.Where(mapping => mapping.ApplicationUserId == idUser)
                    .Include(mapping => mapping.Game)
                    .Include(mapping => mapping.GameSkillLevel)
                    .ToListAsync();

                var userProfileDto = new UserProfileDto()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.ApplicationUserDetails.FirstName,
                    LastName = user.ApplicationUserDetails.LastName,
                    Email = user.Email,
                    DateOfBirth = user.ApplicationUserDetails.DateOfBirth,
                    CountryOfResidence = user.ApplicationUserDetails.CountryOfResidence,
                    Gender = user.ApplicationUserDetails.Gender,
                    PhoneNumber = user.PhoneNumber,
                    GamingPlatforms = gamingPlatforms.Select(mapping => new GamingPlatformDto()
                    {
                        Id = mapping.GamingPlatform.Id,
                        ApiId = mapping.GamingPlatform.ApiId,
                        Abbreviation = mapping.GamingPlatform.Abbreviation,
                        Name = mapping.GamingPlatform.Name,
                        LogoURL = mapping.GamingPlatform.LogoURL
                    }).ToList(),
                    GameGenres = gameGenres.Select(mapping => new GameGenreDto()
                    {
                        Id = mapping.GameGenre.Id,
                        ApiId = mapping.GameGenre.ApiId,
                        Name = mapping.GameGenre.Name,
                        Slug = mapping.GameGenre.Slug
                    }).ToList(),
                    Games = games.Select(mapping => new UserGameDto()
                    {
                        Id = mapping.Game.Id,
                        ApiId = mapping.Game.ApiId,
                        Name = mapping.Game.Name,
                        Summary = mapping.Game.Summary,
                        ReleaseDate = mapping.Game.ReleaseDate,
                        ImageUrl = mapping.Game.ImageUrl,
                        GameSkillLevelId = mapping.GameSkillLevelId,
                        GameSkillLevel = mapping.GameSkillLevel
                    }).ToList()
                };

                return Ok(userProfileDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving user profile information");
            }
        }

        [HttpGet("friendUserIds")]
        public async Task<ActionResult<IEnumerable<string>>> GetFriendUserIds()
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error retrieving the user's friends ids");
                }

                var idUser = GetUserId();
                var friendUserIds = await _context.ApplicationUser_Friends.Where(mapping => mapping.ApplicationUserId == idUser).Select(mapping => mapping.FriendUserId).ToListAsync();

                return Ok(friendUserIds);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving the user's friends ids");
            }
        }

        [HttpGet("activeFriendRequests")]
        public async Task<ActionResult<IEnumerable<FriendRequestDto>>> GetActiveFriendRequests()
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error retrieving the user's active friend requests");
                }

                var idUser = GetUserId();

                var activeFriendRequestStatusType = await _context.FriendRequestStatusTypes.FirstOrDefaultAsync(type => type.EnumCode == (int)Enums.FriendRequestStatusType.Sent);
                var activeFriendRequests = await _context.FriendRequests
                    .Where(request => (request.FromUserId == idUser || request.ToUserId == idUser) && request.FriendRequestStatusId == activeFriendRequestStatusType.Id)
                    .ToListAsync();

                var friendRequestDtos = activeFriendRequests.Select(request => new FriendRequestDto()
                {
                    Id = request.Id,
                    FromUserId = request.FromUserId,
                    ToUserId = request.ToUserId,
                    FriendRequestStatus = activeFriendRequestStatusType
                });

                return Ok(friendRequestDtos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving the user's active friend requests");
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

        [HttpPost("sendFriendRequest")]
        public async Task<ActionResult> SendFriendRequest(FriendRequestDto friendRequestDto)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error sending the friend request");
                }

                var sentFriendRequestStatusType = await _context.FriendRequestStatusTypes.FirstOrDefaultAsync(type => type.EnumCode == (int)Enums.FriendRequestStatusType.Sent);

                if (!_context.FriendRequests.Any(request => request.FromUserId == friendRequestDto.FromUserId && request.ToUserId == friendRequestDto.ToUserId))
                {
                    var newFriendRequest = new FriendRequest()
                    {
                        FromUserId = friendRequestDto.FromUserId,
                        ToUserId = friendRequestDto.ToUserId,
                        FriendRequestStatusId = sentFriendRequestStatusType.Id
                    };

                    _context.FriendRequests.Add(newFriendRequest);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var existingFriendRequest = await _context.FriendRequests.FirstOrDefaultAsync(request => request.FromUserId == friendRequestDto.FromUserId && request.ToUserId == friendRequestDto.ToUserId);
                    existingFriendRequest.FriendRequestStatusId = sentFriendRequestStatusType.Id;

                    _context.Entry(existingFriendRequest).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }

                return StatusCode(StatusCodes.Status202Accepted);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error sending the friend request");
            }
        }

        [HttpPut("cancelFriendRequest")]
        public async Task<ActionResult> CancelFriendRequest(FriendRequestDto friendRequestDto)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error cancelling the friend request");
                }

                var sentFriendRequestStatusType = await _context.FriendRequestStatusTypes.FirstOrDefaultAsync(type => type.EnumCode == (int)Enums.FriendRequestStatusType.Sent);
                var cancelledFriendRequestStatusType = await _context.FriendRequestStatusTypes.FirstOrDefaultAsync(type => type.EnumCode == (int)Enums.FriendRequestStatusType.Cancelled);

                var sentFriendRequest = await _context.FriendRequests.FirstOrDefaultAsync(request => request.FromUserId == friendRequestDto.FromUserId && request.ToUserId == friendRequestDto.ToUserId && request.FriendRequestStatusId == sentFriendRequestStatusType.Id);
                sentFriendRequest.FriendRequestStatusId = cancelledFriendRequestStatusType.Id;

                _context.Entry(sentFriendRequest).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status202Accepted);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error cancelling the friend request");
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

        [HttpPut("search")]
        public async Task<ActionResult<IEnumerable<GamerSearchResult>>> SearchGamers(GamerSearchDto gamerSearch)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error searching for gamers!");
                }

                var idUser = GetUserId();

                var gamersFromUserName = await _context.Users.Include(user => user.ApplicationUserDetails).Where(user => user.UserName.Contains(gamerSearch.SearchCriteria)).ToListAsync();
                var gamersFromFirstName = await _context.Users.Include(user => user.ApplicationUserDetails).Where(user => user.ApplicationUserDetails.FirstName.Contains(gamerSearch.SearchCriteria)).ToListAsync();
                var gamersFromLastName = await _context.Users.Include(user => user.ApplicationUserDetails).Where(user => user.ApplicationUserDetails.LastName.Contains(gamerSearch.SearchCriteria)).ToListAsync();
                var gamersFromEmail = await _context.Users.Include(user => user.ApplicationUserDetails).Where(user => user.Email.Contains(gamerSearch.SearchCriteria)).ToListAsync();

                var gamers = new List<ApplicationUser>();
                gamers.AddRange(gamersFromUserName);
                gamers.AddRange(gamersFromFirstName);
                gamers.AddRange(gamersFromLastName);
                gamers.AddRange(gamersFromEmail);

                var results = gamers.Where(gamer => gamer.Id != idUser).Distinct().Select(gamer => new GamerSearchResult()
                {
                    UserId = gamer.Id,
                    FirstName = gamer.ApplicationUserDetails.FirstName,
                    LastName = gamer.ApplicationUserDetails.LastName,
                    Email = gamer.Email,
                    UserName = gamer.UserName
                });

                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error searching for gamers!");
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
