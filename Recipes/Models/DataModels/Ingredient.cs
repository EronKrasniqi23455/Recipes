using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes.Models.DataModels
{
    public class Ingredient
    {
        
        public Ingredient()
        {
            Recipes = new HashSet<RecipeIngredient>();
        }
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        [Display(Name="Unit")]
        public int UnitId { get; set; }
        //Create relation between Ingridient and Unit table
        [ForeignKey("UnitId")]
        public Unit Unit { get; set; }

        public ICollection<RecipeIngredient> Recipes { get; set; }


    }
}
