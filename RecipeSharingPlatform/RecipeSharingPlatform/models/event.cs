using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using RecipeSharing.Models;

namespace RecipeSharing.models
{
    public class evnet 
    {
        [Key]
        public int Id { get; set; }
        public int Rid { get; set; }
        public string typeEvent { get; set; }
        public string userid { get; set; }
        public string eventDate { get; set; }
        public string? taxt  { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        public Recipe Recipe { get; set; }



    }
}
