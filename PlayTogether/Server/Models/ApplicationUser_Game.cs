using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Server.Models
{
    public class ApplicationUser_Game
    {
        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public int GameId { get; set; }

        public Game Game { get; set; }

        public int? GameSkillLevelId { get; set; }

        public GameSkillLevel GameSkillLevel { get; set; }
    }
}
