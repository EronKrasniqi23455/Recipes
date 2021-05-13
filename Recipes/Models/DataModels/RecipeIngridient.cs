using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes.Models.DataModels
{
    public class RecipeIngridient
    {
        public int RecipeId { get; set; }
        [ForeignKey("RecipeId")]
        public Recipe Recipe { get; set; }

        public int IngridientId { get; set; }
        [ForeignKey("IngridientId")]
        public Ingridient Ingridient { get; set; }

        public double Quantity { get; set; }
    }
}
