using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes.Models.DataModels
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Guid UserId { get; set; }
        public string PictureURL { get; set; }
        
        public int CategoryId { get; set; }

        //Reference to another table as a foreign key
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        //Duration in minutes
        public int Duration { get; set; }
        public string Description { get; set; }
        public List<RecipeIngridient> Ingridients { get; set; }
    }
}
