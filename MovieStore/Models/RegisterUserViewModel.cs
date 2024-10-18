using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MovieStore.Models
{
    public class RegisterUserViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The passwords  do not match.")]
        public string ConfirmPassword { get; set; }

        public string? Role { get; set; }

        [Required(ErrorMessage = "Profile image is required.")]
        public IFormFile ProfileImage { get; set; }
    }
}
