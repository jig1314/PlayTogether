using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PlayTogether.Shared.Models
{
    public class FriendRequestStatusType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EnumCode { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
