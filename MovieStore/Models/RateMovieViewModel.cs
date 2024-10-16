namespace MovieStore.Models
{
    public class RateMovieViewModel
    {
        public int OrderId { get; set; }
        public string MovieTitle { get; set; }
        public List<int> RatingOptions { get; set; } = new List<int> { 1, 2, 3, 4, 5 };
        public int Rating { get; set; } 
    }
}
