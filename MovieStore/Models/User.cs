namespace MovieStore.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // Admin or User

        public string ProfileImagePath { get; set; }

        //public IFormFile ProfileImage { get; set; }

        // Navigation property for the orders created by the user
        public ICollection<Order> Orders { get; set; }
    }

}
