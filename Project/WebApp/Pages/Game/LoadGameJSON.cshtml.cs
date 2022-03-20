using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
    
    public class LoadGameJSON : PageModel
    {
        
        private readonly DAL.ApplicationDbContext _context;

        public LoadGameJSON(DAL.ApplicationDbContext context)
        {
            _context = context;
        }

        public static IList<string> Games { get; set; } = new List<string>()!;

        public void OnGet()
        {
            string workingDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\"));
            String path = workingDirectory + Path.DirectorySeparatorChar + "ConsoleApp" + Path.DirectorySeparatorChar + "SaveGames";
            var files =  Directory.GetFiles(path).ToList();
            foreach (var t in files)
            {
                var arr = t.Split($"{Path.DirectorySeparatorChar}");
                var len = arr.Length;
                Games.Add(WebUtility.UrlDecode(arr[len - 1]));
            }
        }
    }
}