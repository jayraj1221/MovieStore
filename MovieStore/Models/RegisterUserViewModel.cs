using System.ComponentModel.DataAnnotations;

namespace MovieStore.Models
{
    public class RegisterUserViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; } // Either Admin or User

        [Required(ErrorMessage = "Profile image is required.")]
        public IFormFile ProfileImage { get; set; }
    }
}
