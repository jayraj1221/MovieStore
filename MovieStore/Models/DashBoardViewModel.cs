namespace MovieStore.Models
{
    public class DashboardViewModel
    {
        public int TotalMovies { get; set; }
        public int TotalUsers { get; set; }

        public int TotalOrders { get; set; }
        public List<Movie> Movies { get; set; }
        public List<User> Users { get; set; } 

        public List<Movie> TrendingMovies { get; set; }
        public Dictionary<string,int> GenreTransactionData { get; set; }
    }

}
