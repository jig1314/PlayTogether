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
    public class UnauthorizedUserService : IUnauthorizedUserService
    {
        private readonly HttpClient httpClient;

        public UnauthorizedUserService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<Country>> GetCountries() =>
            await httpClient.GetFromJsonAsync<List<Country>>($"api/user/countries");

        public async Task<List<Gender>> GetGenders() =>
            await httpClient.GetFromJsonAsync<List<Gender>>($"api/user/genders");

        public async Task<UserAccountDto> GetUserAccountInfo() =>
            await httpClient.GetFromJsonAsync<UserAccountDto>($"api/user/accountInfo");

        public async Task Login(LoginDto loginDto)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/user/login", loginDto);
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

        public async Task RegisterNewUser(RegisterUserDto registerUserDto)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/user/register", registerUserDto);
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

        public async Task ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/user/resetPassword", resetPasswordDto);
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
    }
}
