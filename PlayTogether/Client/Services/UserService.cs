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
            await httpClient.GetFromJsonAsync<List<GamingPlatformDto>>($"api/gamingPlatforms/userGamingPlatforms");

        public async Task UpdateUserGamingPlatforms(List<GamingPlatformDto> gamingPlatformDtos)
        {
            var response = await httpClient.PutAsJsonAsync("api/gamingPlatforms/updateUserGamingPlatforms", gamingPlatformDtos);
            response.EnsureSuccessStatusCode();
        }
    }
}
