using System;
using System.Collections.Generic;
using System.Text;

namespace PlayTogether.Shared.DTOs
{
    public class UserAccountDto
    {
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int CountryOfResidenceId { get; set; }

        public int GenderId { get; set; }

        public string PhoneNumber { get; set; }
    }
}
