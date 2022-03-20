using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_ShipConfigs
{
    public class EditModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public EditModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ShipConfig ShipConfig { get; set; } = null!;
        public int Id { get; set; } = default;

        public List<string> SuccessMessage { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ShipConfig = await _context.ShipConfigs
                .Include(s => s.Config).FirstOrDefaultAsync(m => m.ShipConfigId == id);

            Id = id.Value;
            
            if (ShipConfig == null)
            {
                return NotFound();
            }
           ViewData["ConfigId"] = new SelectList(_context.Configs, "ConfigId", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(ShipConfig).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShipConfigExists(ShipConfig.ShipConfigId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            SuccessMessage.Add("Successfully updated");
            // return RedirectToPage("./Index");
            return Page();
        }

        private bool ShipConfigExists(int id)
        {
            return _context.ShipConfigs.Any(e => e.ShipConfigId == id);
        }
    }
}
