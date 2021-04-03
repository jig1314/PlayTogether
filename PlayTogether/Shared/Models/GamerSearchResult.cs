using PlayTogether.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlayTogether.Shared.Models
{
    public class GamerSearchResult
    {
        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }
    }
}
