using System.ComponentModel.DataAnnotations;

namespace RecipeSharing.models.auth_model
{
    public class AddRoleModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
