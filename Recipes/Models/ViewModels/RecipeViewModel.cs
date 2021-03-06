using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Recipes.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes.Models.ViewModels
{
    public class RecipeViewModel : Recipe
    {
        //Uploading a picture for recipe
        public IFormFile File{ get; set; }
        public string User { get; set; }
        public bool IsFavourite { get; set; } = false;
        public bool IsOwner { get; set; } = false;
        //Fill the select list with ingridients
        public SelectList IngredientsSelectList { get; set; }
        //Save selected ingrididents as string then they will be processed in controller
        public string IngredientsChoosen { get; set; } = "";
    }
}
