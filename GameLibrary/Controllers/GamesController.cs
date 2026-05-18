using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameLibrary.Data;
using GameLibrary.Models;

namespace GameLibrary.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly AppDbContext context;

        public GamesController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetGames()
        {
            var games = await context.Games.ToListAsync();
            if (games == null || games.Count == 0)
            {
                return Ok(games); // Return empty list instead of NotFound
            }
            return Ok(games);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGame(int id)
        {
            var game = await context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return Ok(game);
        }

        [HttpPost]
        public async Task<ActionResult<Game>> CreateGame(Game game)
        {
            // Check if a game has the same details to prevent duplicates
            var exists = await context.Games.AnyAsync(g => 
                g.Title == game.Title &&
                g.Genre == game.Genre &&
                g.Developer == game.Developer &&
                g.Publisher == game.Publisher &&
                g.ReleaseDate == game.ReleaseDate
            );
            if (exists)
            {
                return BadRequest("Duplicate Entry.");
            }
            context.Games.Add(game);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGame(int id, Game game)
        {
            if (id != game.Id)
            {
                return BadRequest("ID mismatch.");
            }
            
            var exists = await context.Games.AnyAsync(g => g.Id == id);
            if (!exists) {
                return NotFound("Game not found.");
            }

            context.Entry(game).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound("No game to delete.");
            }
            context.Games.Remove(game);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAllGames()
        {
            var games = await context.Games.ToListAsync();
            if (games == null || games.Count == 0)
            {
                return NotFound("No games to delete.");
            }
            context.Games.RemoveRange(games);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}