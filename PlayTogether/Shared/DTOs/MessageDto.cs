using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlayTogether.Shared.DTOs
{
    public class MessageDto
    {
        public string FromUserId { get; set; }

        public UserBasicInfo FromUser { get; set; }
        
        public string ConversationId { get; set; }

        public string MessageText { get; set; }

        public DateTime DateSubmitted { get; set; }
    }
}
