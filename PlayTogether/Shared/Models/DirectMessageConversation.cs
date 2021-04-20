using System;
using System.Collections.Generic;
using System.Text;

namespace PlayTogether.Shared.Models
{
    public class DirectMessageConversation
    {
        public string Id { get; set; }

        public UserBasicInfo WithUser { get; set; }

        public bool HasUnreadMessages { get; set; }
    }
}
