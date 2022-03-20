using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages.Game
{
    public class DeleteGame : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public DeleteGame(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Domain.Game Game { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Game = _context.Games.FirstOrDefault(c => c.GameId == id);

            if (Game == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Game = await _context.Games.FindAsync(id);
            
            if (Game != null)
            {
                List<Domain.Ship> ships1 = _context.Ships.Where(s => s.Ships1GameId == Game.GameId).ToList();
                foreach (var ship in ships1)
                {
                    _context.Ships.Remove(ship);
                }
                List<Domain.Ship> ships2 = _context.Ships.Where(s => s.Ships2GameId == Game.GameId).ToList();
                foreach (var ship in ships2)
                {
                    _context.Ships.Remove(ship);
                }
                _context.Games.Remove(Game);
            
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./load");
        }
    }
}