using System;
using System.Collections.Generic;
using System.Text;

namespace PlayTogether.Shared.DTOs
{
    public class GameGenreDto
    {
        public int Id { get; set; }

        public long ApiId { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }
    }
}
