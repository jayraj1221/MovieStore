using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MovieStore.Models
{
    public class AddMovieModel
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Genre is required.")]
        public string SelectedGenre { get; set; } // The genre selected by the user

        [Required(ErrorMessage = "Release date is required.")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, 1000, ErrorMessage = "Price must be between 0 and 1000.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Movie poster is required.")]
        public IFormFile PosterImage { get; set; } // For image upload
    }

}
