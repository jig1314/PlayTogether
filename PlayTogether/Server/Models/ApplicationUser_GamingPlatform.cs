using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Server.Models
{
    public class ApplicationUser_GamingPlatform
    {
        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public int GamingPlatformId { get; set; }

        public GamingPlatform GamingPlatform { get; set; }
    }
}
