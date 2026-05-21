using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GameLibrary.Data;
using GameLibrary.Models;
using GameLibrary.Models.DTOs;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

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
        [Authorize]
        public async Task<ActionResult<LibraryEntry>> CreateLibraryEntry(LibraryEntryDto entryDto)
        {
            // Create a new library entry
            var entry = new LibraryEntry
            {
                UserId = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!),
                GameId = entryDto.GameId,
                Status = entryDto.Status,
                User = null!,
                Game = null!,
                AddedDate = DateTime.UtcNow
            };
        
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

            // Add the new entry to the database
            context.LibraryEntries.Add(entry);
            await context.SaveChangesAsync();

            // Return the created entry
            LibraryEntryResponseDto responseDto = new LibraryEntryResponseDto
            {
                Id = entry.Id,
                UserId = entry.UserId,
                GameId = entry.GameId,
                AddedDate = entry.AddedDate,
                GameTitle = entry.Game.Title
            };

            return CreatedAtAction(nameof(GetLibraryEntry), new { id = entry.Id }, responseDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLibraryEntry(int id, LibraryEntryDto entryDto)
        {
            // Load the library entry to update
            var entry = await context.LibraryEntries.FindAsync(id);
            if (entry == null)
            {
                return NotFound("Library Entry not found.");
            }

            entry.GameId = entryDto.GameId;
            entry.Status = entryDto.Status;

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