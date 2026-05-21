using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibrary.Models
{
    public class LibraryEntry
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GameId { get; set; }
        public User User { get; set; } = null!;
        public Game Game { get; set; } = null!;
        public int Status { get; set; }
        public DateTime AddedDate { get; set; }
    }
}