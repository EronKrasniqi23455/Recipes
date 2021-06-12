using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;
using Recipes.Models.DataModels;
using Recipes.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Grpc.Core;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Recipes.Controllers
{
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<IdentityUser> _userManager;
        private IHostingEnvironment hostingEnv;


        public RecipesController(ApplicationDbContext context, Microsoft.AspNetCore.Identity.UserManager<IdentityUser> userManager, IHostingEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            this.hostingEnv = env;
        }

        // GET: Recipes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Recipes.Include(x => x.Category).ToList();
            var dataViewModel = new List<RecipeViewModel>();
            foreach (var item in applicationDbContext)
            {
                var tempUser = _context.Users.FirstOrDefault(x => x.Id == item.UserId.ToString());
                dataViewModel.Add(new RecipeViewModel()
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    User = tempUser.Email,
                    PictureURL = item.PictureURL,
                    Duration = item.Duration,
                    Category = item.Category,
                    IsOwner = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) == item.UserId ? true : false
                });
            }
            return View(dataViewModel);
        }

        #region Details
        // GET: Recipes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .Include(r=>r.Ingredients)
                .FirstOrDefaultAsync(m => m.Id == id);

            if(recipe.Ingredients != null)
            {
                foreach (var item in recipe.Ingredients)
                {
                    item.Ingredient = _context.Ingredients.Include(x => x.Unit).FirstOrDefault(x => x.Id == item.IngredientId);
                }
            }

            var tempUser = _context.Users.FirstOrDefault(x => x.Id == recipe.UserId.ToString());
            var response = new RecipeViewModel()
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                User = tempUser.Email,
                PictureURL = recipe.PictureURL,
                Duration = recipe.Duration,
                Category = recipe.Category,
                Ingredients = recipe.Ingredients,
                IsOwner = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) == recipe.UserId ? true : false
            };
            if (recipe == null)
            {
                return NotFound();
            }

            return View(response);
        }
        #endregion

        #region Create
        // GET: Recipes/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Title");
            RecipeViewModel model = new RecipeViewModel();
            // model.Ingredients = await _context.Ingredients.Include(x => x.Unit).ToListAsync();
            model.IngredientsSelectList = new SelectList(_context.Ingredients.Include(x => x.Unit), "Id", "Name");
            return View(model);
        }

        // POST: Recipes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecipeViewModel recipe)
        {
            if (ModelState.IsValid)
            {
                recipe.UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (recipe.File != null)
                {
                    Random rnd = new Random();
                    int month = rnd.Next(2323232, 949494943);
                    string ImagePath = month + System.IO.Path.GetFileName(recipe.File.FileName);
                    var FileDic = "Files";
                    string physicalPath = Path.Combine(hostingEnv.WebRootPath, FileDic);
                    var t = Path.Combine(physicalPath, ImagePath);
                    using (FileStream fs = System.IO.File.Create(t))
                    {
                        recipe.File.CopyTo(fs);
                    }
                    recipe.PictureURL = ImagePath;
                }
                var ingridientsChoosen = recipe.IngredientsChoosen.Split('|');
                _context.Add(recipe);
                await _context.SaveChangesAsync();
                var recipeId = recipe.Id;
                var ingridients = GetIngridientsList(ingridientsChoosen, recipeId);
                await _context.RecipeIngredients.AddRangeAsync(ingridients);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Title", recipe.CategoryId);
            return View(recipe);
        }

        #endregion

        #region Edit
        // GET: Recipes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes.FindAsync(id);

            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (recipe.UserId != currentUserId)
            {
                ModelState.AddModelError(string.Empty, "You are not authorized to edit this item");
                return View();
            }
            if (recipe == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", recipe.CategoryId);
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Recipe recipe)
        {
            if (id != recipe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipe.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Id", recipe.CategoryId);
            return View(recipe);
        }

        #endregion

        #region Delete
        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Favorites 
        public async Task<IActionResult> Favorites()
        {
            var currentUser = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var favorites = await _context.Recipes.Include(x => x.Favorites).Where(x => x.Favorites.Any(x => x.UserId == currentUser)).ToListAsync();
            var dataViewModel = new List<RecipeViewModel>();
            foreach (var item in favorites)
            {
                var tempUser = _context.Users.FirstOrDefault(x => x.Id == item.UserId.ToString());
                dataViewModel.Add(new RecipeViewModel()
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    User = tempUser.Email,
                    PictureURL = item.PictureURL,
                    Duration = item.Duration,
                    Category = item.Category,
                    IsOwner = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) == item.UserId ? true : false
                });
            }
            return View(dataViewModel);
        }

        #endregion

        #region My Recipes
        public async Task<IActionResult> MyRecipes()
        {
            var currentUser = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var myRecipes = await _context.Recipes.Where(x => x.UserId == currentUser).ToListAsync();
            var dataViewModel = new List<RecipeViewModel>();
            foreach (var item in myRecipes)
            {
                var tempUser = _context.Users.FirstOrDefault(x => x.Id == item.UserId.ToString());
                dataViewModel.Add(new RecipeViewModel()
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    User = tempUser.Email,
                    PictureURL = item.PictureURL,
                    Duration = item.Duration,
                    Category = item.Category,
                    IsOwner = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) == item.UserId ? true : false
                });
            }
            return View(dataViewModel);
        }
        #endregion

        #region Methods 
        private List<RecipeIngredient> GetIngridientsList(string[] ingridientsInArrayForm, int recipeId)
        {
            var listIngridients = new List<RecipeIngredient>();
            foreach (var item in ingridientsInArrayForm)
            {
                if (!String.IsNullOrEmpty(item
                    ))
                {
                    var data = item.Split(',');
                    listIngridients.Add(new RecipeIngredient()
                    {
                        RecipeId = recipeId,
                        IngredientId = int.Parse(data[0]),
                        Quantity = double.Parse(data[1])
                    });
                }
            }
            return listIngridients;
        }
        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }
        #endregion
    }
}
