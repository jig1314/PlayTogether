using PlayTogether.Shared.DTOs;
using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Client.Services
{
    public interface IUserService
    {
        Task<List<Gender>> GetGenders();
        Task<List<Country>> GetCountries();

        Task<UserAccountDto> GetUserAccountInfo();
        Task UpdateUserAccountInfo(UserAccountDto userAccountDto);
        Task UpdatePassword(ChangePasswordDto changePasswordDto);

        Task<List<GamingPlatformDto>> GetUserGamingPlatforms();
        Task AddUserGamingPlatform(GamingPlatformDto gamingPlatform);
        Task RemoveUserGamingPlatform(GamingPlatformDto gamingPlatform);

        Task<List<GameGenreDto>> GetUserGameGenres();
        Task UpdateUserGameGenres(List<int> gameGenreIds);

        Task<List<UserGameDto>> GetUserGames();
        Task AddUserGame(UserGameDto game);
        Task RemoveUserGame(long apiId);
        Task UpdateUserGameSkillLevel(UserGameDto game);
        Task<List<UserBasicInfo>> SearchForGamers(GamerSearchDto gamerSearchDto);
        Task<UserBasicInfo> GetUserBasicInformation(string userName);
        Task<UserProfileDto> GetUserProfileInformation(string userName);
        Task<List<UserBasicInfo>> GetFriends();
        Task<List<FriendRequestDto>> GetActiveFriendRequests();
        Task SendFriendRequest(FriendRequestDto friendRequest);
        Task CancelFriendRequest(FriendRequestDto cancelledFriendRequest);
        Task DeclineFriendRequest(FriendRequestDto declinedFriendRequest);
        Task AcceptFriendRequest(FriendRequestDto acceptedFriendRequest);
        Task<List<UserBasicInfo>> GetUsersFriends(string userName);
        Task UnfriendUser(string idUser);
        Task DeleteAccount(DeleteAccountDto deleteAccountDto);
    }
}
