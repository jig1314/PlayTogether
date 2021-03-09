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

        public async Task Login(LoginDto loginDto)
        {
            var response = await httpClient.PostAsJsonAsync("api/user/login", loginDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task RegisterNewUser(RegisterUserDto registerUserDto)
        {
            var response = await httpClient.PostAsJsonAsync("api/user/register", registerUserDto);
            response.EnsureSuccessStatusCode();
        }
    }
}
