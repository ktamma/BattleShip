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

namespace WebApp.Pages_Configs
{
    public class DeleteModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public DeleteModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Config Config { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Config = await _context.Configs
                .Include(c => c.TouchRule).FirstOrDefaultAsync(m => m.ConfigId == id);

            if (Config == null)
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

            Config = await _context.Configs.FindAsync(id);

            if (Config != null)
            {
                List<Domain.ShipConfig> ships = _context.ShipConfigs.Where(s => s.ConfigId == Config.ConfigId).ToList();
                foreach (var shipConfig in ships)
                {
                    _context.ShipConfigs.Remove(shipConfig);
                }
                _context.Configs.Remove(Config);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
