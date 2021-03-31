using PlayTogether.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Client.ViewModels
{
    public class GameSearchViewModel
    {
        public string SearchCriteria { get; set; }

        public List<GamingPlatformDto> GamingPlatformDtos { get; set; }

        public List<GameGenreDto> GameGenreDtos { get; set; }
    }
}
