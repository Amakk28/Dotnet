using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GameLibrary.Data;
using GameLibrary.Models;

namespace GameLibrary.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LibraryEntryController : ControllerBase
    {
        private readonly AppDbContext context;

        public LibraryEntryController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LibraryEntry>>> GetLibraryEntries()
        {
            var entries = await context.LibraryEntries
                .Include(le => le.User)
                .Include(le => le.Game)
                .ToListAsync();

            return Ok(entries);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LibraryEntry>> GetLibraryEntry(int id)
        {
            var entry = await context.LibraryEntries
                .Include(le => le.User)
                .Include(le => le.Game)
                .FirstOrDefaultAsync(le => le.Id == id);

            if (entry == null)
            {
                return NotFound();
            }
            return Ok(entry);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<LibraryEntry>>> GetLibraryEntriesByUser(int userId)
        {
            var entries = await context.LibraryEntries
                .Include(le => le.User)
                .Include(le => le.Game)
                .Where(le => le.UserId == userId)
                .ToListAsync();

            return Ok(entries);
        }

        [HttpPost]
        public async Task<ActionResult<LibraryEntry>> CreateLibraryEntry(LibraryEntry entry)
        {
            // Check if the user and game exist
            var userExists = await context.Users.AnyAsync(u => u.Id == entry.UserId);
            var gameExists = await context.Games.AnyAsync(g => g.Id == entry.GameId);

            if (!userExists || !gameExists)
            {
                return NotFound("User or Game not found.");
            }

            // Check for duplicate entry
            var exists = await context.LibraryEntries.AnyAsync(le =>
                le.UserId == entry.UserId && le.GameId == entry.GameId);
            if (exists)
            {
                return BadRequest("Duplicate Entry.");
            }

            context.LibraryEntries.Add(entry);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLibraryEntry), new { id = entry.Id }, entry);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLibraryEntry(int id, LibraryEntry entry)
        {
            if (id != entry.Id)
            {
                return BadRequest("ID mismatch.");
            }

            var exists = await context.LibraryEntries.AnyAsync(le => le.Id == id);
            if (!exists) {
                return NotFound("Library Entry not found.");
            }

            context.Entry(entry).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLibraryEntry(int id)
        {
            var entry = await context.LibraryEntries.FindAsync(id);
            if (entry == null)
            {
                return NotFound("Library Entry not found.");
            }

            context.LibraryEntries.Remove(entry);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}