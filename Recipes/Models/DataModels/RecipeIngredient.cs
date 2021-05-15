using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes.Models.DataModels
{
    public class RecipeIngredient
    {
        [Key, Column(Order =0) ]
        public int RecipeId { get; set; }
        [ForeignKey("RecipeId")]
        public Recipe Recipe { get; set; }

        [Key,Column(Order =1)]
        public int IngredientId { get; set; }
        [ForeignKey("IngridientId")]
        public Ingredient Ingredient { get; set; }
        
        public double Quantity { get; set; }
    }
}
