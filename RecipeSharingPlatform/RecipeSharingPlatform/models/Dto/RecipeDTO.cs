

using Microsoft.AspNetCore.Identity;
using RecipeSharing.models;


namespace RecipeSharing.Models
{
    public class RecipeDTO
    {
        public string Name { get; set; } 
        public string? Instructions { get; set; }
        public int ? CookingTime { get; set; } 
        public string? categories { get; set; }
        public string? CuisineOrigin { get; set; }

        public int? Calories { get; set; }
        public int? Fat { get; set; } 
        public int? Carbohydrates { get; set; } 
        public int? Protein { get; set; } 
        public string Ingredients { get; set; } 
       

    }
}

