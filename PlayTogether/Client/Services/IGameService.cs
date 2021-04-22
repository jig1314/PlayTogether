using PlayTogether.Shared.DTOs;
using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Client.Services
{
    public interface IGameService
    {
        Task<List<GameSearchResult>> SearchForGames(GameSearchDto gameSearchDto);
        Task<List<GamingPlatformDto>> GetGamingPlatforms();
        Task<List<GameGenreDto>> GetGameGenres();
        Task<List<GameSkillLevelDto>> GetGameSkillLevels();
    }
}
