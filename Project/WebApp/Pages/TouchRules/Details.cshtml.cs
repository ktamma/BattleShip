using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_TouchRules
{
    public class DetailsModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public DetailsModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public TouchRule TouchRule { get; set; }= null!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TouchRule = await _context.TouchRules.FirstOrDefaultAsync(m => m.TouchRuleId == id);

            if (TouchRule == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
