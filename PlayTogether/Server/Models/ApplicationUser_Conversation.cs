using System;
using PlayTogether.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Server.Models
{
    public class ApplicationUser_Conversation
    {
        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public string ConversationId { get; set; }

        public Conversation Conversation { get; set; }
    }
}
