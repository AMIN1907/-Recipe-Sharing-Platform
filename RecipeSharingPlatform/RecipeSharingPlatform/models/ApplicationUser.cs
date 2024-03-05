
using RecipeSharing.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RecipeSharing.Controllers;
using RecipeSharing.Models;

namespace RecipeSharing.models
{
    public class ApplicationUser : IdentityUser

    {
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public string Gender { get; set; }
        public string BIO { get; set; }
        public string? ImageUrl { get; set; }

       
        public ICollection<Followers> Followers { get; set; }

        
        public ICollection<Following> Following { get; set; }
        public ICollection<Recipe> Recipes { get; set; }
       
        public ICollection<evnet> evnets { get; set; }



    }

}
