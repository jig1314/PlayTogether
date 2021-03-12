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
    }
}
