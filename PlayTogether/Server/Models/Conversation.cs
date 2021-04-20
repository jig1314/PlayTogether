using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PlayTogether.Server.Models
{
    public class Conversation
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string CreatedByUserId { get; set; }

        public ApplicationUser CreatedByUser { get; set; }

        public List<Message> Messages { get; set; }

        public List<ApplicationUser_Conversation> Users { get; set; }
    }
}
