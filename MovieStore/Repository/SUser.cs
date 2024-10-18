using Microsoft.EntityFrameworkCore;
using MovieStore.Models;
using System.Text.RegularExpressions;

namespace MovieStore.Repository
{
    public class SUser : IUser
    {
        private readonly MovieStoreContext context;
        public SUser(MovieStoreContext _context) {
            context = _context;
        }

        public async Task<User> RegisterAsync(User user)
        {
            // You may want to hash the password before saving it to the database
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            // In a real application, you'd want to verify the hashed password
            return await context.Users.SingleOrDefaultAsync(u => u.UserName == username && u.Password == password);
        }

        public  User GetUserByIdAsync(int id)
        {
            return  context.Users.Find(id);
        }

        public void AddOrder(Order order)
        {
            // Add the order to the Orders table
            context.Orders.Add(order);
            context.SaveChanges();
        }
        public User GetUserByUsername(string userName)
        {
            return context.Users.FirstOrDefault(u => u.UserName == userName);
        }

        public bool HasUserPurchasedMovie(int userId, int movieId)
        {
            return context.Orders.Any(o => o.UserId == userId && o.MovieId == movieId);
        }

        public IEnumerable<Order> GetOrdersByUserId(int userId)
        {
            return context.Orders
                           .Where(o => o.UserId == userId)
                           .Include(o => o.Movie) // Assuming you want to include movie details
                           .ToList();
        }

        public void UpdateOrder(Order order)
        {
            context.Orders.Update(order);
            context.SaveChanges();
        }

        public Order GetOrderById(int orderId)
        {
            return context.Orders.Include(o => o.Movie).FirstOrDefault(o => o.OrderId == orderId);
        }
        public void UpdateUser(User user)
        {
            context.Users.Update(user);
            context.SaveChanges();
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
            var movies = context.Movies
               .AsEnumerable()  // Switch to client-side evaluation
               .Where(m => regex.IsMatch(m.Title) || regex.IsMatch(m.Genre))
               .ToList();

            return movies;
        }
        public Movie GetMovieById(int id)
        {
            return context.Movies.FirstOrDefault(m => m.MovieId == id);
        }
        public  bool UserNameExistsAsync(string userName)
        {
            return  context.Users.Any(u => u.UserName == userName);
        }

        public  bool EmailExistsAsync(string email)
        {
            return  context.Users.Any(u => u.Email == email);
        }
    }
}
