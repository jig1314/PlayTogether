using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Server.Models
{
    public class GamingPlatform_Game
    {
        public int GamingPlatformId { get; set; }

        public GamingPlatform GamingPlatform { get; set; }

        public int GameId { get; set; }

        public Game Game { get; set; }
    }
}
