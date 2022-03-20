using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_Games
{
    public class LoadModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public LoadModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Game> Game { get;set; }= null!;

        public async Task OnGetAsync()
        {
            Game = await _context.Games.ToListAsync();
        }
    }
}
