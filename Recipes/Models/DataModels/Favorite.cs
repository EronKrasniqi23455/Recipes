using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes.Models.DataModels
{
    public class Favorite
    {
        //one user can make one favorite recipe only once, not many times
        //for example if we like one cake we can make that recipe favvorite only once
        [Key, Column(Order =0)]
        public Guid UserId { get; set; }

        [Key, Column(Order =1)]
        public int RecipeId { get; set; }
        //create relation between favorite and recipeId
        [ForeignKey("RecipeId")]
        public Recipe Recipe { get; set; }

        
    }
}
