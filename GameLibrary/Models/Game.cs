using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibrary.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Genre { get; set; }
        public string? Developer { get; set; }
        public string? Publisher { get; set; }
        public string? CoverImageUrl { get; set; }
        public DateTime? ReleaseDate { get; set; }
    }
}