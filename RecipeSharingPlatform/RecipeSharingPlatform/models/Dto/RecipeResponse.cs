using RecipeSharing.Models;

namespace RecipeSharing.models.Dto
{
    public class RecipeResponse
    {    
        public int id { get; set; }
        public string Name { get; set; }
        public string? categories { get; set; }
        public string CuisineOrigin { get; set; }

        public string Instructions { get; set; }
        public int CookingTime { get; set; }
        public int? Calories { get; set; }
        public int? Fat { get; set; }
        public int? Carbohydrates { get; set; }
        public int? Protein { get; set; }
        public string Ingredients { get; set; }
        public string? RecipeImageUrl { get; set; }
        public int? Totallike { get; set; }
        public int? totaltasty { get; set; }
        public int? Totaldislike { get; set; }
        public int? Totalcomment { get; set; }
        public int? Totalshere { get; set; }

        public string BIO { get; set; }
            public string? Caption { get; set; }
        public string? UserName { get; set; }
        public string? userImageUrl { get; set; }
        public RecipeResponse oRecipeResponse { get; set; }


    }
}
