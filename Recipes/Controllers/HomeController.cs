using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Recipes.Data;
using Recipes.Models;
using Recipes.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var applicationDbContext = _context.Recipes.Include(x => x.Category).ToList();
            var dataViewModel = new List<RecipeViewModel>();
            foreach (var item in applicationDbContext)
            {
                var tempUser = _context.Users.FirstOrDefault(x => x.Id == item.UserId.ToString());
                dataViewModel.Add(new RecipeViewModel()
                {
                    Title = item.Title,
                    Description = item.Description,
                    User = tempUser.Email,
                    PictureURL = item.PictureURL,
                    Duration = item.Duration
                });
            }                                                         
            return View(dataViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
