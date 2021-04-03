using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Shared.DTOs
{
    public class UserProfileDto
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Country CountryOfResidence { get; set; }

        public Gender Gender { get; set; }

        public string PhoneNumber { get; set; }

        public List<GamingPlatformDto> GamingPlatforms { get; set; }

        public List<GameGenreDto> GameGenres { get; set; }

        public List<UserGameDto> Games { get; set; }
    }
}
