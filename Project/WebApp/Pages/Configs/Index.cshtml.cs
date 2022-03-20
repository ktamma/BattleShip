using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_Configs
{
    public class IndexModel : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public IndexModel(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Config> Config { get; set; } = null!;

        public async Task OnGetAsync(string? action, int? id)
        {
            Config = await _context.Configs
                .Include(c => c.TouchRule).ToListAsync();
            if (action == "setConfig")
            {
                if (id != null)
                {
                    await using var db = new ApplicationDbContext();
                    var conf = db.Configs.First(m => m.ConfigId == id);
                    var touchRule = db.TouchRules.First(m => m.TouchRuleId == conf.TouchRuleId);
                    var shipConfigs = db.ShipConfigs.Where(m => m.ConfigId == conf.ConfigId).ToList();


                    var config = Pages.Game.Index.GameBrain.GetConfigFromDbObject(conf, shipConfigs, touchRule);
                    Pages.Game.Index.GameConfig = config;


                    Pages.Game.Index.GameBrain = new BLL.GameBrain(config);
                }
            }
        }
    }
}