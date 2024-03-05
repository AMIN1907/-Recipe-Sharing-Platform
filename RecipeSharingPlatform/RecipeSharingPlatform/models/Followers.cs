namespace RecipeSharing.models
{
    public class Followers
    {

        // Foreign keys
        public string ApplicationUser1Id { get; set; }
        public string ApplicationUser2Id { get; set; }

        // Navigation properties
        public ApplicationUser ApplicationUser { get; set; }

    }
}
