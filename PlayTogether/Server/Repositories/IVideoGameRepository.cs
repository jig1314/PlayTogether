using PlayTogether.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Server.Repositories
{
    public interface IVideoGameRepository
    {
        Task<List<GameGenre>> GetGameGenresAsync();
    }
}
