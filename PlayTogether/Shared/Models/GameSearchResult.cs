using PlayTogether.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlayTogether.Shared.Models
{
    public class GameSearchResult
    {
        public long ApiId { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string ImageUrl { get; set; }

        public int ImageHeight { get; set; }

        public int ImageWidth { get; set; }

        public List<GamingPlatformDto> GamingPlatforms { get; set; }

        public List<GameGenreDto> GameGenres { get; set; }
    }
}
