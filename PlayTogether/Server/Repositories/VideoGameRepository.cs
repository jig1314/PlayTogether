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

            var query = new StringBuilder(" fields *; limit 500; where first_release_date != null");

            if (gameSearch.GameGenreApiIds?.Count > 0)
            {
                query.AppendFormat($" & genres = ({string.Join(',', gameSearch.GameGenreApiIds)})");
            }

            if (gameSearch.GamingPlatformApiIds?.Count > 0)
            {
                query.AppendFormat($" & platforms = ({string.Join(',', gameSearch.GamingPlatformApiIds)})");
            }

            query.AppendFormat($"; ");

            if (!string.IsNullOrWhiteSpace(gameSearch.SearchCriteria))
            {
                query.AppendFormat($" search \"{gameSearch.SearchCriteria}\"; ");
            }

            var games = await client.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: query.ToString());
            var gameCovers = await client.QueryAsync<Cover>(IGDBClient.Endpoints.Covers, query: $" fields *; limit 500; where id = ({string.Join(',', games.Where(game => game.Cover != null && game.Id.HasValue).Select(game => game.Cover.Id))}); ");

            foreach (var game in games.Where(game => game.Cover != null && game.Cover.Id.HasValue))
            {
                game.Cover = new IdentityOrValue<Cover>(gameCovers.FirstOrDefault(cover => cover.Id == game.Cover.Id.Value));
            }

            return games;
        }

        public async Task<Game> GetGameAsync(long apiId)
        {
            var apiHeaders = await GetHeaders();
            var client = new IGDBClient(apiHeaders.clientId, apiHeaders.authorization);

            var query = $" fields *; limit 1; where id = {apiId}; ";

            var game = (await client.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: query)).ToList().FirstOrDefault();

            if (game.Cover != null && game.Cover.Id.HasValue)
            {
                var gameCover = await client.QueryAsync<Cover>(IGDBClient.Endpoints.Covers, query: $" fields *; limit 1; where id = {game.Cover.Id}; ");
                game.Cover = new IdentityOrValue<Cover>(gameCover.ToList().FirstOrDefault());
            }

            return game;
        }
    }
}
