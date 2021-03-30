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

        Task<List<GameDto>> GetUserGames();
        Task AddUserGame(GameDto game);
        Task RemoveUserGame(long apiId);
    }
}
