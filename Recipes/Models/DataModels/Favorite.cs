using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes.Models.DataModels
{
    public class Favorite
    {
        public Guid UserId { get; set; }

        public int RecipeId { get; set; }
    }
}
