namespace RecipeSharing.models
{
    public class Following
    {

        // Foreign keys
        public string ApplicationUser1Id { get; set; }
        public string ApplicationUser2Id { get; set; }

        // Navigation properties
        public ApplicationUser ApplicationUser { get; set; }

    }
}
