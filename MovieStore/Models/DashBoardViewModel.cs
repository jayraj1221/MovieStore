namespace MovieStore.Models
{
    public class DashboardViewModel
    {
        public int TotalMovies { get; set; }
        public int TotalUsers { get; set; }
        public List<Movie> Movies { get; set; }
        public List<User> Users { get; set; } 
    }

}
