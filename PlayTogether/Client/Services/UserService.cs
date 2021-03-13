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
            var response = await httpClient.PutAsJsonAsync("api/user/updateAccountInfo", userAccountDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdatePassword(ChangePasswordDto changePasswordDto)
        {
            var response = await httpClient.PutAsJsonAsync("api/user/changePassword", changePasswordDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<GamingPlatformDto>> GetGamingPlatforms() =>
            await httpClient.GetFromJsonAsync<List<GamingPlatformDto>>($"api/gamingPlatforms/gamingPlatforms");

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

        public async Task<List<GameGenreDto>> GetGameGenres() =>
            await httpClient.GetFromJsonAsync<List<GameGenreDto>>($"api/gameGenres/gameGenres");

        public async Task<List<GameGenreDto>> GetUserGameGenres() =>
            await httpClient.GetFromJsonAsync<List<GameGenreDto>>($"api/gameGenres/userGameGenres");

        public async Task UpdateUserGameGenres(List<int> gameGenreIds)
        {
            var response = await httpClient.PutAsJsonAsync("api/gameGenres/updateGameGenres", gameGenreIds);
            response.EnsureSuccessStatusCode();
        }
    }
}
