

using Microsoft.AspNetCore.Identity;
using RecipeSharing.models;


namespace RecipeSharing.Models
    {
        public class Recipe
        {
            public int RecipeId { get; set; } // Primary key
            public string UserId { get; set; } // Foreign key for the user who created the recipe

            public string? Name { get; set; } // Name of the recipe
            public string? Ingredients { get; set; } // Ingredients
            public string? Instructions { get; set; } // Cooking instructions
            public int? CookingTime { get; set; } // Cooking time
            public string? categories { get; set; } 
            public string? CuisineOrigin { get; set; }

            public string? ImageUrl { get; set; } // Photo URL

            public int? Calories { get; set; } // Total calories per serving
            public int? Fat { get; set; } // Total fat content per serving (in grams)
            public int? Carbohydrates { get; set; } // Total carbohydrates per serving (in grams)
            public int? Protein { get; set; } // Total protein content per serving (in grams)

          
            public bool Shared { get; set; } = false;
            public string? Caption { get; set; }

            public int  OriginaRecipeid { get; set; }
           public DateTime? createdate { get; set; }
 

         public ApplicationUser ApplicationUser { get; set; }
          public ICollection<evnet> evnets { get; set; }

    }
}

