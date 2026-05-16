using Microsoft.EntityFrameworkCore;
using GameLibrary.Models;

namespace GameLibrary.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<LibraryEntry> LibraryEntries { get; set; } = null!;        
    }
}