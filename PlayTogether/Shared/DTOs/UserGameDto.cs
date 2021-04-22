using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlayTogether.Shared.DTOs
{
    public class UserGameDto
    {
        public int? Id { get; set; }

        public long ApiId { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string ImageUrl { get; set; }

        public int? GameSkillLevelId { get; set; }

        public GameSkillLevelDto GameSkillLevel { get; set; }
    }
}
