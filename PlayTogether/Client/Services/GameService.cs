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
    public class GameService : IGameService
    {
        private readonly HttpClient httpClient;

        public GameService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<GameGenreDto>> GetGameGenres() =>
            await httpClient.GetFromJsonAsync<List<GameGenreDto>>($"api/gameGenres/gameGenres");

        public async Task<List<GamingPlatformDto>> GetGamingPlatforms() =>
            await httpClient.GetFromJsonAsync<List<GamingPlatformDto>>($"api/gamingPlatforms/gamingPlatforms");

        public async Task<List<GameSearchResult>> SearchForGames(GameSearchDto gameSearchDto)
        {
            var response = await httpClient.PutAsJsonAsync("api/games/search", gameSearchDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<GameSearchResult>>();
        }
    }
}
