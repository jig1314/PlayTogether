using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlayTogether.Shared.DTOs
{
    public class FriendRequestDto
    {
        public long? Id { get; set; }

        public string FromUserId { get; set; }

        public string ToUserId { get; set; }

        public FriendRequestStatusType FriendRequestStatus { get; set; }
    }
}
