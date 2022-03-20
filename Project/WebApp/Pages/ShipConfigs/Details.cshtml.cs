using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_ShipConfigs
{
    public class DetailsModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public DetailsModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public ShipConfig ShipConfig { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ShipConfig = await _context.ShipConfigs
                .Include(s => s.Config).FirstOrDefaultAsync(m => m.ShipConfigId == id);

            if (ShipConfig == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
