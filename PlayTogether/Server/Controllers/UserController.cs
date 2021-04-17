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

        [HttpGet("getFriends")]
        public async Task<ActionResult<IEnumerable<UserBasicInfo>>> GetFriends()
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error retrieving the user's friends");
                }

                var idUser = GetUserId();
                var friendUsers = await _context.ApplicationUser_Friends
                    .Include(mapping => mapping.FriendUser)
                    .Include(mapping => mapping.FriendUser.ApplicationUserDetails)
                    .Where(mapping => mapping.ApplicationUserId == idUser).Select(mapping => new UserBasicInfo()
                    {
                        UserId = mapping.FriendUserId,
                        FirstName = mapping.FriendUser.ApplicationUserDetails.FirstName,
                        LastName = mapping.FriendUser.ApplicationUserDetails.LastName,
                        Email = mapping.FriendUser.Email,
                        UserName = mapping.FriendUser.UserName
                    }).ToListAsync();

                return Ok(friendUsers);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving the user's friends");
            }
        }

        [HttpGet("getFriends/{userName}")]
        public async Task<ActionResult<IEnumerable<UserBasicInfo>>> GetFriends(string userName)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error retrieving the user's friends");
                }

                var friendUsers = await _context.ApplicationUser_Friends
                    .Include(mapping => mapping.ApplicationUser)
                    .Include(mapping => mapping.FriendUser)
                    .Include(mapping => mapping.FriendUser.ApplicationUserDetails)
                    .Where(mapping => mapping.ApplicationUser.UserName == userName).Select(mapping => new UserBasicInfo()
                    {
                        UserId = mapping.FriendUserId,
                        FirstName = mapping.FriendUser.ApplicationUserDetails.FirstName,
                        LastName = mapping.FriendUser.ApplicationUserDetails.LastName,
                        Email = mapping.FriendUser.Email,
                        UserName = mapping.FriendUser.UserName
                    }).ToListAsync();

                return Ok(friendUsers);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving the user's friends");
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
                    .Include(request => request.FromUser).Include(request => request.FromUser.ApplicationUserDetails)
                    .Include(request => request.ToUser).Include(request => request.ToUser.ApplicationUserDetails)
                    .Where(request => (request.FromUserId == idUser || request.ToUserId == idUser) && request.FriendRequestStatusId == activeFriendRequestStatusType.Id)
                    .ToListAsync();

                var friendRequestDtos = activeFriendRequests.Select(request => new FriendRequestDto()
                {
                    Id = request.Id,
                    FromUserId = request.FromUserId,
                    FromUser = new UserBasicInfo()
                    {
                        UserId = request.FromUser.Id,
                        FirstName = request.FromUser.ApplicationUserDetails.FirstName,
                        LastName = request.FromUser.ApplicationUserDetails.LastName,
                        Email = request.FromUser.Email,
                        UserName = request.FromUser.UserName
                    },
                    ToUserId = request.ToUserId,
                    ToUser = new UserBasicInfo()
                    {
                        UserId = request.ToUser.Id,
                        FirstName = request.ToUser.ApplicationUserDetails.FirstName,
                        LastName = request.ToUser.ApplicationUserDetails.LastName,
                        Email = request.ToUser.Email,
                        UserName = request.ToUser.UserName
                    },
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
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error cancelling the friend request");
            }
        }

        [HttpPut("acceptFriendRequest")]
        public async Task<ActionResult> AcceptFriendRequest(FriendRequestDto friendRequestDto)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error declining the friend request");
                }

                var sentFriendRequestStatusType = await _context.FriendRequestStatusTypes.FirstOrDefaultAsync(type => type.EnumCode == (int)Enums.FriendRequestStatusType.Sent);
                var acceptedFriendRequestStatusType = await _context.FriendRequestStatusTypes.FirstOrDefaultAsync(type => type.EnumCode == (int)Enums.FriendRequestStatusType.Accepted);

                var sentFriendRequest = await _context.FriendRequests.FirstOrDefaultAsync(request => request.FromUserId == friendRequestDto.FromUserId && request.ToUserId == friendRequestDto.ToUserId && request.FriendRequestStatusId == sentFriendRequestStatusType.Id);
                sentFriendRequest.FriendRequestStatusId = acceptedFriendRequestStatusType.Id;

                var newFriends = new List<ApplicationUser_Friend>()
                {
                    new ApplicationUser_Friend() { ApplicationUserId = friendRequestDto.FromUserId, FriendUserId = friendRequestDto.ToUserId },
                    new ApplicationUser_Friend() { ApplicationUserId = friendRequestDto.ToUserId, FriendUserId = friendRequestDto.FromUserId }
                };

                _context.Entry(sentFriendRequest).State = EntityState.Modified;
                _context.ApplicationUser_Friends.AddRange(newFriends);
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status202Accepted);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error declining the friend request");
            }
        }


        [HttpPut("declineFriendRequest")]
        public async Task<ActionResult> DeclineFriendRequest(FriendRequestDto friendRequestDto)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error declining the friend request");
                }

                var sentFriendRequestStatusType = await _context.FriendRequestStatusTypes.FirstOrDefaultAsync(type => type.EnumCode == (int)Enums.FriendRequestStatusType.Sent);
                var rejectedFriendRequestStatusType = await _context.FriendRequestStatusTypes.FirstOrDefaultAsync(type => type.EnumCode == (int)Enums.FriendRequestStatusType.Rejected);

                var sentFriendRequest = await _context.FriendRequests.FirstOrDefaultAsync(request => request.FromUserId == friendRequestDto.FromUserId && request.ToUserId == friendRequestDto.ToUserId && request.FriendRequestStatusId == sentFriendRequestStatusType.Id);
                sentFriendRequest.FriendRequestStatusId = rejectedFriendRequestStatusType.Id;

                _context.Entry(sentFriendRequest).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status202Accepted);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error declining the friend request");
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
        public async Task<ActionResult<IEnumerable<UserBasicInfo>>> SearchGamers(GamerSearchDto gamerSearch)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error searching for gamers!");
                }

                var idUser = GetUserId();
                var gamers = new List<ApplicationUser>();

                if (!string.IsNullOrWhiteSpace(gamerSearch.SearchCriteria))
                {
                    gamers = await _context.Users
                        .Include(user => user.ApplicationUserDetails)
                        .Include(user => user.GamingPlatforms)
                        .Include(user => user.GameGenres)
                        .Include(user => user.Games)
                        .Where(user => user.UserName.Contains(gamerSearch.SearchCriteria) ||
                            user.ApplicationUserDetails.FirstName.Contains(gamerSearch.SearchCriteria) ||
                            user.ApplicationUserDetails.LastName.Contains(gamerSearch.SearchCriteria) ||
                            user.Email.Contains(gamerSearch.SearchCriteria)).ToListAsync();
                }
                else
                {
                    gamers = await _context.Users
                        .Include(user => user.ApplicationUserDetails)
                        .Include(user => user.GamingPlatforms)
                        .Include(user => user.GameGenres)
                        .Include(user => user.Games).ToListAsync();
                }

                if (gamerSearch.GamingPlatformIds?.Count > 0)
                {
                    gamers = gamers.Where(user => user.GamingPlatforms.Any(platform => gamerSearch.GamingPlatformIds.Contains(platform.GamingPlatformId))).ToList();
                }

                if (gamerSearch.GameGenreIds?.Count > 0)
                {
                    gamers = gamers.Where(user => user.GameGenres.Any(genre => gamerSearch.GameGenreIds.Contains(genre.GameGenreId))).ToList();
                }

                if (gamerSearch.GameIds?.Count > 0)
                {
                    gamers = gamers.Where(user => user.Games.Any(game => gamerSearch.GameIds.Contains(game.GameId))).ToList();
                }

                var results = gamers.Where(gamer => gamer.Id != idUser).Distinct().Select(gamer => new UserBasicInfo()
                {
                    UserId = gamer.Id,
                    FirstName = gamer.ApplicationUserDetails.FirstName,
                    LastName = gamer.ApplicationUserDetails.LastName,
                    Email = gamer.Email,
                    UserName = gamer.UserName
                });

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, (ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
            }
        }

        [HttpPut("deleteAccount")]
        public async Task<ActionResult> DeleteAccount(DeleteAccountDto deleteAccountDto)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "You are not authorized to delete this account!");
                }

                var idUser = GetUserId();
                var user = await _context.Users
                    .Include(user => user.ApplicationUserDetails)
                    .Include(user => user.GamingPlatforms)
                    .Include(user => user.GameGenres)
                    .Include(user => user.Games)
                    .Include(user => user.SentFriendRequests)
                    .Include(user => user.ReceivedFriendRequests)
                    .FirstOrDefaultAsync(user => user.Id == idUser);

                var password = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(deleteAccountDto.Password));
                if (!await _userManager.CheckPasswordAsync(user, password))
                {
                    throw new Exception($"Incorrect password.");
                }

                var friendships = await _context.ApplicationUser_Friends.Where(mapping => mapping.ApplicationUserId == idUser || mapping.FriendUserId == idUser).ToListAsync();
                _context.ApplicationUser_Friends.RemoveRange(friendships);
                _context.FriendRequests.RemoveRange(user.ReceivedFriendRequests);
                _context.FriendRequests.RemoveRange(user.SentFriendRequests);
                _context.ApplicationUser_Games.RemoveRange(user.Games);
                _context.ApplicationUser_GameGenres.RemoveRange(user.GameGenres);
                _context.ApplicationUser_GamingPlatform.RemoveRange(user.GamingPlatforms);
                _context.ApplicationUserDetails.RemoveRange(user.ApplicationUserDetails);

                await _context.SaveChangesAsync();

                var result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Unexpected error occurred deleting the account for {user.UserName}.");
                }

                await _signInManager.SignOutAsync();

                return StatusCode(StatusCodes.Status202Accepted);
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

        [HttpDelete("unfriendUser/{friendUserId}")]
        public async Task<ActionResult> UnfriendUser(string friendUserId)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Error unfriending user!");
                }

                var idUser = GetUserId();
                var friendships = await _context.ApplicationUser_Friends.Where(mapping => (mapping.ApplicationUserId == idUser && mapping.FriendUserId == friendUserId) || (mapping.ApplicationUserId == friendUserId && mapping.FriendUserId == idUser)).ToListAsync();

                _context.ApplicationUser_Friends.RemoveRange(friendships);
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status202Accepted);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, (ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}
