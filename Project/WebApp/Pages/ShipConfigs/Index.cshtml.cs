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
    public class IndexModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public IndexModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ShipConfig> ShipConfig { get;set; } = null!;

        public async Task OnGetAsync(int? id)
        {
            if (id != null)
            {
                ShipConfig = await _context.ShipConfigs
                    .Include(s => s.Config).Where(s => s.ConfigId == id!.Value).ToListAsync();
            }
            else
            {
                ShipConfig = await _context.ShipConfigs
                    .Include(s => s.Config).ToListAsync();
            }
            
        }
    }
}
