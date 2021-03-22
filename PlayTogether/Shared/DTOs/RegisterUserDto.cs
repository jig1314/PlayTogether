using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlayTogether.Shared.DTOs
{
    public class RegisterUserDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string Email { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public DateTime DateOfBirth { get; set; }
        
        public int CountryOfResidenceId { get; set; }

        public int GenderId { get; set; }
    }
}
