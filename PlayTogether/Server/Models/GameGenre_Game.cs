using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Server.Models
{
    public class GameGenre_Game
    {
        public int GameGenreId { get; set; }

        public GameGenre GameGenre { get; set; }

        public int GameId { get; set; }

        public Game Game { get; set; }
    }
}
