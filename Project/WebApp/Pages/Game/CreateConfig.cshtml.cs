using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using BLL;
using BLL.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using Domain;

namespace WebApp.Pages.Game
{
    public class CreateConfig : PageModel
    {
        private readonly DAL.ApplicationDbContext _context;

        public CreateConfig(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["TouchRuleId"] = new SelectList(_context.TouchRules, "TouchRuleId", "Rule");
            ShipConfigs.Add(new Domain.ShipConfig
            {
                Name = "Patrol",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 1
            });
            ShipConfigs.Add(new Domain.ShipConfig
            {
                Name = "Cruiser",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 2,
            });
            ShipConfigs.Add(new Domain.ShipConfig
            {
                Name = "Submarine",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 3,
            });
            ShipConfigs.Add(new Domain.ShipConfig
            {
                Name = "Battleship",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 4
            });
            ShipConfigs.Add(new Domain.ShipConfig
            {
                Name = "Carrier",
                Quantity = 1,
                ShipSizeY = 1,
                ShipSizeX = 5,
            });
            return Page();
        }

        [BindProperty] public Config Config { get; set; } = null!;
        [BindProperty] public List<ShipConfig> ShipConfigs { get; set; } = new List<ShipConfig>();

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Request.Form["JSONsave"].Equals("save to JSON")) //JSON save
            {
                var saveName = Config.Name;
                var gameBrain = new GameBrain(new GameConfig());
                var touchRule = _context.TouchRules.First(r => r.TouchRuleId == Config.TouchRuleId);
                var conf = gameBrain.GetConfigFromDbObject(Config, ShipConfigs, touchRule);
                string workingDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\"));
                string path = workingDirectory + Path.DirectorySeparatorChar + "ConsoleApp" +
                              Path.DirectorySeparatorChar + "Configs" + Path.DirectorySeparatorChar +
                              WebUtility.UrlEncode(saveName) + ".json";
                Console.WriteLine(path);

                var jsonOptions = new JsonSerializerOptions()
                {
                    WriteIndented = true
                };
           

            var confJsonStr = JsonSerializer.Serialize(conf, jsonOptions);


                System.IO.File.WriteAllText(path, confJsonStr);
            }

            if (Request.Form["saveToDatabase"].Equals("save to database"))
            {

                _context.Configs.Add(Config);
                
                await _context.SaveChangesAsync();
                foreach (var shipConfig in ShipConfigs)
                {
                    Console.WriteLine(shipConfig.Quantity);
                    shipConfig.ConfigId = Config.ConfigId;
                    _context.ShipConfigs.Add(shipConfig);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("../Index");
            }
        }
    }

