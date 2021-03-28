using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PlayTogether.Server.Models
{
    public class GamingPlatform
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ApiId { get; set; }

        public string Abbreviation { get; set; }

        [Required]
        public string Name { get; set; }

        public string Slug { get; set; }

        public string LogoURL { get; set; }

        public List<ApplicationUser_GamingPlatform> Users { get; set; }

        public List<GamingPlatform_Game> Games { get; set; }
    }
}
