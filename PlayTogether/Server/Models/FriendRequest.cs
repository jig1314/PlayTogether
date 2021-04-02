using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Server.Models
{
    public class FriendRequest
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string FromUserId { get; set; }

        public ApplicationUser FromUser { get; set; }

        [Required]
        public string ToUserId { get; set; }

        public ApplicationUser ToUser { get; set; }

        [Required]
        public int FriendRequestStatusId { get; set; }

        public FriendRequestStatusType FriendRequestStatus { get; set; }
    }
}
