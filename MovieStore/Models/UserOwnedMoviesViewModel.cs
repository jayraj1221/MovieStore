namespace MovieStore.Models
{
    public class UserOwnedMoviesViewModel
    {
        public User user { get; set; }
        public IEnumerable<Movie> OwnedMovies { get; set; }
        public decimal TotalSpent { get; set; }
        public Dictionary<string, List<Movie>> MoviesByGenre { get; set; }
    }
}
