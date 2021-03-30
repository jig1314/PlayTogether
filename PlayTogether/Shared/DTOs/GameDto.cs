using System;
using System.Collections.Generic;
using System.Text;

namespace PlayTogether.Shared.DTOs
{
    public class GameDto
    {
        public int? Id { get; set; }

        public long ApiId { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string ImageUrl { get; set; }
    }
}
