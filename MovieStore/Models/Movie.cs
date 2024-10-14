namespace MovieStore.Models
{
    public class Movie
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public DateTime ReleaseDate { get; set; }
        public decimal Price { get; set; }

        public string ImagePath { get; set; }
        // Navigation property for the orders that contain this movie
        public ICollection<OrderItem> OrderItems { get; set; }
    }

}
