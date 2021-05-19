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
            var applicationDbContext = _context.Recipes.Include(x=>x.Category).ToList();
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
                    Category = item.Category
                });
            }
            return View(dataViewModel);
        }

        // GET: Recipes/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Recipes/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Title");
            return View();
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
                _context.Add(recipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Title", recipe.CategoryId);
            return View(recipe);
        }

        // GET: Recipes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes.FindAsync(id);
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

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }
    }
}
