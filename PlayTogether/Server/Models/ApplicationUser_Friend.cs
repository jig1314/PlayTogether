using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Server.Models
{
    public class ApplicationUser_Friend
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public string FriendUserId { get; set; }

        public ApplicationUser FriendUser { get; set; }
    }
}
