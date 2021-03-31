using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Server.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public long ApiId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Summary { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        public string ImageUrl { get; set; }

        public List<ApplicationUser_Game> Users { get; set; }

        public List<GamingPlatform_Game> GamingPlatforms { get; set; }

        public List<GameGenre_Game> GameGenres { get; set; }


    }
}
