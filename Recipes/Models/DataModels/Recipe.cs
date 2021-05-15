using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes.Models.DataModels
{
    public class Recipe
    {
        public Recipe()
        {
            Ingredients=new HashSet<RecipeIngredient>();
            Favorites = new HashSet<Favorite>();
        }
        


        //mandatory key(preferable)
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public Guid UserId { get; set; }

        public string PictureURL { get; set; }  
        
        //Duration in minutes
        public int Duration { get; set; }

        public string Description { get; set; }

        //list changed to icollection
        public ICollection<RecipeIngredient> Ingredients { get; set; }

        

        public int CategoryId { get; set; }
        //Reference to another table as a foreign key
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public ICollection<Favorite> Favorites { get; set; }
    }
}
