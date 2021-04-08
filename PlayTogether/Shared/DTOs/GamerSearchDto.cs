using System;
using System.Collections.Generic;
using System.Text;

namespace PlayTogether.Shared.DTOs
{
    public class GamerSearchDto
    {
        public string SearchCriteria { get; set; }

        public List<int> GamingPlatformIds { get; set; }

        public List<int> GameGenreIds { get; set; }

        public List<int> GameIds { get; set; }
    }
}
