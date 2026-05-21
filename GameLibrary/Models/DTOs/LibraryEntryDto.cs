using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibrary.Models.DTOs
{
    public class LibraryEntryDto
    {
        [Required]
        public int GameId { get; set; }
        public int Status { get; set; } 
    }

    public class LibraryEntryResponseDto
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int UserId { get; set; }
        public DateTime AddedDate { get; set; }
        public string GameTitle { get; set; } = string.Empty;
    }
}