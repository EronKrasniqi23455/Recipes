using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes.Models.DataModels
{
    public class RecipeIngridient
    {
        public int RecipeId { get; set; }

        public int IngridientId { get; set; }

        public double Quantity { get; set; }
    }
}
