using System;
using System.Collections.Generic;
using System.Text;

namespace PlayTogether.Shared.DTOs
{
    public class GamingPlatformDto
    {
        public int Id { get; set; }

        public int ApiId { get; set; }

        public string Abbreviation { get; set; }

        public string Name { get; set; }

        public string LogoURL { get; set; }
    }
}
