using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUserDetails ApplicationUserDetails { get; set; }

        public List<ApplicationUser_GamingPlatform> GamingPlatforms { get; set; }

        public List<ApplicationUser_GameGenre> GameGenres { get; set; }

        public List<ApplicationUser_Game> Games { get; set; }

        public List<FriendRequest> SentFriendRequests { get; set; }

        public List<FriendRequest> ReceivedFriendRequests { get; set; }

        public List<ApplicationUser_Friend> Friends { get; set; }

        public List<Message> SentMessages { get; set; }

        public List<ApplicationUser_MessageConnection> MessageConnections { get; set; }

        public List<ApplicationUser_Conversation> Conversations { get; set; }
    }
}