using System;
using System.Collections.Generic;
using System.Text;

namespace PlayTogether.Shared.Models
{
    public class ChatGroupConversation
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string CreatedByUserId { get; set; }

        public List<UserBasicInfo> WithUsers { get; set; }

        public bool HasUnreadMessages { get; set; }
    }
}
