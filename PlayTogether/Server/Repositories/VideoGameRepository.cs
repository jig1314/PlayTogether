using IGDB;
using IGDB.Models;
using Microsoft.EntityFrameworkCore;
using PlayTogether.Server.Data;
using PlayTogether.Shared.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<IEnumerable<Game>> GetGamesAsync(GameSearchDto gameSearch)
        {
            var apiHeaders = await GetHeaders();
            var client = new IGDBClient(apiHeaders.clientId, apiHeaders.authorization);

            var query = new StringBuilder(" fields *; limit 500; ");

            if (!string.IsNullOrWhiteSpace(gameSearch.SearchCriteria))
            {
                query.AppendFormat($" search \"{gameSearch.SearchCriteria}\"; ");
            }

            if (gameSearch.GameGenreApiIds?.Count > 0)
            {
                query.AppendFormat($" where genres = ({string.Join(',', gameSearch.GameGenreApiIds)}); ");
            }

            if (gameSearch.GamingPlatformApiIds?.Count > 0)
            {
                query.AppendFormat($" where platforms = ({string.Join(',', gameSearch.GamingPlatformApiIds)}); ");
            }

            var games = await client.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: query.ToString());

            return games;
        }

        public async Task<Game> GetGameAsync(long apiId)
        {
            var apiHeaders = await GetHeaders();
            var client = new IGDBClient(apiHeaders.clientId, apiHeaders.authorization);

            var query = $" fields *; limit 1; where id = {apiId}; ";

            var results = await client.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: query);
            return results.ToList().FirstOrDefault();
        }
    }
}
