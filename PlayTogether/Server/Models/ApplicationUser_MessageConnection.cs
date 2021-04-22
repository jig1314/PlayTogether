using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Server.Models
{
    public class ApplicationUser_MessageConnection
    {
        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public string MessageConnectionId { get; set; }

        public MessageConnection MessageConnection { get; set; }
    }
}
