using Microsoft.EntityFrameworkCore;
using MovieStore.Models;

namespace MovieStore.Repository
{
    public interface IMovie
    {
        List<Movie> GetAllMovies();
        Movie GetMovieById(int id);
        void AddMovie(Movie movie);
        void UpdateMovie(Movie movie);
        void DeleteMovie(int id);
        public List<User> GetAllUsers();
        public int GetTotalMovies();
        public int GetTotalUsers();
        public int GetTotalOrders();
        IEnumerable<Movie> SearchMovies(string query);
        Dictionary<string, int> GetGenreTransactionDataAsync();
        public IEnumerable<Movie> GetUserOwnedMovies(int userId);
        public decimal GetTotalMoneySpentByUser(int userId);
        public Dictionary<string, List<Movie>> GetMoviesByGenre(int userId);
        public IEnumerable<User> GetUsersOwningMovie(int movieId);

        public  List<Movie> GetTrendingMovies();

        public Dictionary<string, List<Movie>> GetMoviesByGenreAll();

        public IEnumerable<Order> GetAllOrders();

        public List<int> GetMovieReviews(int movieId);

        public bool IsMoviePurchased(int movieId);

    }
}
