using System.ComponentModel.DataAnnotations;

namespace RecipeSharing.models.auth_model
{
    public class TokenRequestModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; } 
    }
}
