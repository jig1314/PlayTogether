using PlayTogether.Shared.DTOs;
using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PlayTogether.Client.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient httpClient;

        public UserService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<Country>> GetCountries() =>
            await httpClient.GetFromJsonAsync<List<Country>>($"api/user/countries");

        public async Task<List<Gender>> GetGenders() =>
            await httpClient.GetFromJsonAsync<List<Gender>>($"api/user/genders");

        public async Task<UserAccountDto> GetUserAccountInfo() =>
            await httpClient.GetFromJsonAsync<UserAccountDto>($"api/user/accountInfo");

        public async Task UpdateUserAccountInfo(UserAccountDto userAccountDto)
        {
            try
            {
                var response = await httpClient.PutAsJsonAsync("api/user/updateAccountInfo", userAccountDto);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(content);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdatePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {
                var response = await httpClient.PutAsJsonAsync("api/user/changePassword", changePasswordDto);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(content);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<GamingPlatformDto>> GetUserGamingPlatforms() =>
            await httpClient.GetFromJsonAsync<List<GamingPlatformDto>>($"api/gamingPlatforms/userGamingPlatforms");

        public async Task AddUserGamingPlatform(GamingPlatformDto gamingPlatform)
        {
            var response = await httpClient.PostAsJsonAsync("api/gamingPlatforms/addUserGamingPlatform", gamingPlatform);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveUserGamingPlatform(GamingPlatformDto gamingPlatform)
        {
            var response = await httpClient.DeleteAsync($"api/gamingPlatforms/deleteUserGamingPlatform/{gamingPlatform.Id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<GameGenreDto>> GetUserGameGenres() =>
            await httpClient.GetFromJsonAsync<List<GameGenreDto>>($"api/gameGenres/userGameGenres");

        public async Task UpdateUserGameGenres(List<int> gameGenreIds)
        {
            var response = await httpClient.PutAsJsonAsync("api/gameGenres/updateGameGenres", gameGenreIds);
            response.EnsureSuccessStatusCode();
        }

        public async Task AddUserGame(UserGameDto game)
        {
            var response = await httpClient.PostAsJsonAsync("api/games/addUserGame", game);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<UserGameDto>> GetUserGames() =>
            await httpClient.GetFromJsonAsync<List<UserGameDto>>($"api/games/userGames");

        public async Task RemoveUserGame(long apiId)
        {
            var response = await httpClient.DeleteAsync($"api/games/deleteUserGame/{apiId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateUserGameSkillLevel(UserGameDto game)
        {
            var response = await httpClient.PutAsJsonAsync("api/games/updateUserGameSkillLevel", game);
            response.EnsureSuccessStatusCode();
        }
    }
}
