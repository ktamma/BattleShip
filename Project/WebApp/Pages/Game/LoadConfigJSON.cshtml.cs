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
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace WebApp.Pages.Game
{
    
    public class LoadConfigJSON : PageModel
    {
        
        private readonly DAL.ApplicationDbContext _context;

        public LoadConfigJSON(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public static IList<string> Configs { get; set; } = new List<string>()!;

        public void OnGet(string? action, int? id)
        {
            string workingDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\"));
            String path = workingDirectory + Path.DirectorySeparatorChar + "ConsoleApp" + Path.DirectorySeparatorChar + "Configs";
            Console.WriteLine(path);
            var files =  Directory.GetFiles(path).ToList();
            foreach (var t in files)
            {
                var arr = t.Split($"{Path.DirectorySeparatorChar}");
                var len = arr.Length;
                Configs.Add(WebUtility.UrlDecode(arr[len - 1]));
            }
            
            if(action=="setConfigFromJSON")
            {
                var fileName = files[id!.Value];

                Console.WriteLine(fileName);
                var confText = System.IO.File.ReadAllText(fileName);
                Index.GameConfig = JsonSerializer.Deserialize<GameConfig>(confText) ?? new GameConfig();
                Index.GameBrain = new BLL.GameBrain(Index.GameConfig);
                
            }
        }
    }
}