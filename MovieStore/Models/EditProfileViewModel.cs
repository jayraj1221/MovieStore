using System.ComponentModel.DataAnnotations;

namespace MovieStore.Models
{
    public class EditProfileViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        public IFormFile? ProfileImage { get; set; } // For uploading a new profile picture (optional)

        public string? ProfileImagePath { get; set; } 
    }
}
