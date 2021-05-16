using Microsoft.AspNetCore.Http;
using Recipes.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes.Models.ViewModels
{
    public class RecipeViewModel : Recipe
    {
        public IFormFile File{ get; set; }
        public string User { get; set; }
    }
}
