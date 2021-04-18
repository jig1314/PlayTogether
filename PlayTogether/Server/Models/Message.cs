using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Server.Models
{
    public class Message
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string FromUserId { get; set; }

        public ApplicationUser FromUser { get; set; }

        [Required]
        public string ConversationId { get; set; }

        public Conversation Conversation { get; set; }

        [Required]
        public string MessageText { get; set; }
    }
}
