using System;
using System.Collections.Generic;
using System.Text;

namespace PlayTogether.Shared.DTOs
{
    public class GameSearchDto
    {
        public string SearchCriteria { get; set; }

        public List<int> GamingPlatformApiIds { get; set; }

        public List<int> GameGenreApiIds { get; set; }
    }
}
