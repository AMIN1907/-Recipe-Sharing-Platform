namespace RecipeSharing.models.Dto
{
    public class ProfileDto
   
    {    
       
        public string? fname { get; set; }
        public string? lname { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public DateTime barthday { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ginder { get; set;}
        public string BIO { get; set; }
        public string? ImageUrl { get; set; }
        public int following { get; set; }
        public int followers { get; set; }

    }
}
