using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PlayTogether.Server.Models
{
    public class MessageConnection
    {
        [Key]
        public string ConnectionId { get; set; }

        public string UserAgent { get; set; }

        public bool Connected { get; set; }

        public List<ApplicationUser_MessageConnection> Users { get; set; }
    }
}
