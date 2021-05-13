using System;
using System.Collections.Generic;
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
        public int Duration { get; set; }
        public string Description { get; set; }
    }
}
