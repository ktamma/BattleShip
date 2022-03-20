using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using Domain;

namespace WebApp.Pages_Configs
{
    public class CreateModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public CreateModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["TouchRuleId"] = new SelectList(_context.TouchRules, "TouchRuleId", "Rule");
            return Page();
        }

        [BindProperty] public Config Config { get; set; } = null!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Configs.Add(Config);
            
            await _context.SaveChangesAsync();
            _context.ShipConfigs.Add(new Domain.ShipConfig
            {
                ConfigId = Config.ConfigId,
                Name = "Patrol",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 1
            });
            _context.ShipConfigs.Add(new Domain.ShipConfig
            {
                ConfigId = Config.ConfigId,
                Name = "Cruiser",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 2,
            });
            _context.ShipConfigs.Add(new Domain.ShipConfig
            {
                ConfigId = Config.ConfigId,
                Name = "Submarine",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 3,
            });
            _context.ShipConfigs.Add(new Domain.ShipConfig
            {
                ConfigId = Config.ConfigId,
                Name = "Battleship",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 4
            });
            _context.ShipConfigs.Add(new Domain.ShipConfig
            {
                ConfigId = Config.ConfigId,
                Name = "Carrier",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 5,
            });
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
