using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Recipes.Data;
using Recipes.Models;
using Recipes.Models.DataModels;
using Recipes.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
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

        [HttpGet]
        public IActionResult Index([FromQuery] int? recipeId)
        {
            if (recipeId != null)
                Favourite((int)recipeId);

            var applicationDbContext = _context.Recipes.Include(x => x.Category).ToList();
            var dataViewModel = new List<RecipeViewModel>();
            foreach (var item in applicationDbContext)
            {
                var tempUser = _context.Users.FirstOrDefault(x => x.Id == item.UserId.ToString());
                var tempItem = new RecipeViewModel()
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    User = tempUser.Email,
                    PictureURL = item.PictureURL,
                    Duration = item.Duration
                };

                if (User.Identity.IsAuthenticated)
                {
                    //Get logged in UserId
                    var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    tempItem.IsFavourite = _context.Favorites.FirstOrDefault(x => x.RecipeId == item.Id && x.UserId == currentUserId) != null ? true : false;
                }
                dataViewModel.Add(tempItem);
            }
            return View(dataViewModel);
        }

        public bool Favourite(int recipeId)
        {
            var currentUser = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var ifExists = _context.Favorites.Where(x => x.RecipeId == recipeId && x.UserId == currentUser).Count() > 0 ? true : false;

            //If the user has already made a recipe favourite, then remove that from the favorites list
            if (ifExists)
            {
                var favouriteTempObj = _context.Favorites.FirstOrDefault(x => x.RecipeId == recipeId && x.UserId == currentUser);
                _context.Favorites.Remove(favouriteTempObj);
            }
            else
            {
                //if user wants to make a recipe favorite
                var favourite = new Favorite()
                {
                    RecipeId = recipeId,
                    UserId = currentUser
                };
                _context.Favorites.Add(favourite);
            }
            _context.SaveChanges();
            return true;
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
