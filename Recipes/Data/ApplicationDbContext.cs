using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Recipes.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recipes.Data
{
    //This class is an important class in entity framework, is a connection between your classes and database
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Favorite>().HasKey(table => new {
                table.UserId,
                table.RecipeId
            });
            builder.Entity<RecipeIngredient>().HasKey(table => new { 
                table.IngredientId,
                table.RecipeId
            });
        }
        //To create tables into our database via migrations 
        public DbSet<Category> Categories{ get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Ingredient> Ingredients{ get; set; }
        public DbSet<Recipe> Recipes{ get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients{ get; set; }
        public DbSet<Unit> Units{ get; set; }

    }
}
