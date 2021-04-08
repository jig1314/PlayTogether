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

        public string FromUserId { get; set; }

        public ApplicationUser FromUser { get; set; }

        public string ToUserId { get; set; }

        public ApplicationUser ToUser { get; set; }

        public int FriendRequestStatusId { get; set; }
    }
}
