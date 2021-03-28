using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Server.Models
{
    public class GameGenre
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ApiId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Slug { get; set; }

        public List<ApplicationUser_GameGenre> Users { get; set; }

        public List<GameGenre_Game> Games { get; set; }
    }
}