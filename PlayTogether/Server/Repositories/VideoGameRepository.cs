using IGDB;
using IGDB.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlayTogether.Server.Data;
using PlayTogether.Server.Enums;
using PlayTogether.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Server.Repositories
{
    public class VideoGameRepository : IVideoGameRepository
    {
        private readonly ApplicationDbContext _context;

        public VideoGameRepository(ApplicationDbContext context)
        {
            this._context = context;
        }

        private async Task<(string clientId, string authorization)> GetHeaders()
        {
            var clientId = (await _context.AppSettings.FirstOrDefaultAsync(setting => setting.EnumCode == (int)Enums.AppSetting.IgdbClientId)).Value;
            var authorization = (await _context.AppSettings.FirstOrDefaultAsync(setting => setting.EnumCode == (int)Enums.AppSetting.IgdbAuthorization)).Value;

            return (clientId, authorization);
        }

        public async Task<List<GameGenre>> GetGameGenresAsync()
        {
            var apiHeaders = await GetHeaders();
            var client = new IGDBClient(apiHeaders.clientId, apiHeaders.authorization);
            var genres = await client.QueryAsync<Genre>(IGDBClient.Endpoints.Genres, query: "fields *; limit 200;");

            var gameGenres = genres.Select(genre => new GameGenre()
            {
                Name = genre.Name,
                Slug = genre.Slug
            }).ToList();

            return gameGenres;
        }
    }
}
