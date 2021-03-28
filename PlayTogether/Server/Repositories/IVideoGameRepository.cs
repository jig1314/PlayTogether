using IGDB.Models;
using PlayTogether.Shared.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlayTogether.Server.Repositories
{
    public interface IVideoGameRepository
    {
        Task<IEnumerable<Game>> GetGamesAsync(GameSearchDto gameSearch);

        Task<Game> GetGameAsync(long apiId);
    }
}
