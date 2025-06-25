using MovieStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace MovieStore.Repository
{
    public class MovieRepository : IMovie
    {
        private readonly MovieStoreContext _context;

        public MovieRepository(MovieStoreContext context)
        {
            _context = context;
        }
        public int GetTotalMovies()
        {
            return _context.Movies.Count(); // Assuming Movies is your DbSet<Movie>
        }

        public int GetTotalUsers()
        {
            return _context.Users.Count(); // Assuming Users is your DbSet<User>
        }
        public int GetTotalOrders()
        {
            return _context.Orders.Count(); // Assuming Users is your DbSet<User>
        }


        public List<Movie> GetAllMovies()
        {
            return _context.Movies.ToList();
        }

        public Movie GetMovieById(int id)
        {
            return _context.Movies.Find(id);
        }

        public void AddMovie(Movie movie)
        {
            _context.Movies.Add(movie);
            _context.SaveChanges(); // Make sure to save changes to the database
        }

        public void UpdateMovie(Movie movie)
        {
            _context.Entry(movie).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteMovie(int id)
        {
            var movie = GetMovieById(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                _context.SaveChanges();
            }
        }
        public List<User> GetAllUsers()
        {
            return _context.Users.ToList(); // Adjust based on your context
        }


        public IEnumerable<Movie> SearchMovies(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return new List<Movie>();
            }

            // Compile a case-insensitive regular expression from the query
            Regex regex;
            try
            {
                regex = new Regex(query, RegexOptions.IgnoreCase);
            }
            catch (RegexParseException)
            {
                // Handle invalid regex patterns (return empty list or handle accordingly)
                return new List<Movie>();
            }

            // Filter movies based on Title or Genre using regex matching
            var movies = _context.Movies
               .AsEnumerable()  // Switch to client-side evaluation
               .Where(m => regex.IsMatch(m.Title) || regex.IsMatch(m.Genre))
               .ToList();

            return movies;
        }
        // Method to get total movies
        public  Dictionary<string, int> GetGenreTransactionDataAsync()
        {
            // Fetch all orders including related Movie entities
            var orders =  _context.Orders.Include(o => o.Movie).ToList();

            // Group by genre and count transactions per genre
            var genreTransactionData = orders
                .GroupBy(o => o.Movie.Genre)
                .ToDictionary(g => g.Key, g => g.Count());

            return genreTransactionData;
        }

        public IEnumerable<Movie> GetUserOwnedMovies(int userId)
        {
            return _context.Orders
                .Where(mu => mu.UserId == userId)
                .Include(mu => mu.Movie)
                .Select(mu => mu.Movie)
                .ToList();
        }

        public decimal GetTotalMoneySpentByUser(int userId)
        {
            return _context.Orders
                .Where(mu => mu.UserId == userId)
                .Sum(mu => mu.Movie.Price);
        }

        public Dictionary<string, List<Movie>> GetMoviesByGenre(int userId)
        {
            return _context.Orders
                .Where(mu => mu.UserId == userId)
                .Include(mu => mu.Movie)
                .GroupBy(mu => mu.Movie.Genre)
                .ToDictionary(g => g.Key, g => g.Select(mu => mu.Movie).ToList());
        }
        public IEnumerable<User> GetUsersOwningMovie(int movieId)
        {
            return _context.Orders
                .Where(o => o.MovieId == movieId)
                .Select(o => o.User)
                .ToList();
        }
        public  List<Movie> GetTrendingMovies()
        {
            var trendingMovies =  _context.Orders
             .GroupBy(o => o.MovieId) // Group by MovieId
             .Select(g => new
             {
                 MovieId = g.Key,
                 AverageRating = g.Average(o => o.Rating) // Calculate average rating
             })
             .OrderByDescending(m => m.AverageRating) // Order by average rating descending
             .Take(5) // Get the top 5 trending movies
             .ToList();

            // Get the movie details for the trending movies
            var movieIds = trendingMovies.Select(m => m.MovieId).ToList();
            var movies =  _context.Movies
                .Where(m => movieIds.Contains(m.MovieId))
                .ToList();

            return movies;
        }
        public Dictionary<string, List<Movie>> GetMoviesByGenreAll()
        {
            return _context.Movies
                    .GroupBy(m => m.Genre)
                    .ToDictionary(g => g.Key, g => g.ToList());
        }
        public IEnumerable<Order> GetAllOrders()
        {
            return _context.Orders
                .Include(o => o.Movie)
                .Include(o=>o.User)// Assuming each order is linked to a movie
                .ToList();
        }

        public List<int> GetMovieReviews(int movieId)
        {
            
            var reviews = _context.Orders
                .Where(o => o.MovieId == movieId && o.Rating > 0) 
                .Select(o => o.Rating) 
                .ToList(); 

            return reviews;
        }
        public bool IsMoviePurchased(int movieId)
        {
            return _context.Orders.Any(o => o.MovieId == movieId);
        }

    }
}