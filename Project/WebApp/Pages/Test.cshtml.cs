using System.Collections.Generic;
using System.Linq;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
    public class Test : PageModel
    {
        private readonly ApplicationDbContext _ctx;

        public Test(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }


        public List<Domain.Game> Games { get; set; } = default!;
        public void OnGet()
        {
            Games = _ctx.Games.ToList();
        }
    }
}