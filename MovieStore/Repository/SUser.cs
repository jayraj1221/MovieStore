using Microsoft.EntityFrameworkCore;
using MovieStore.Models;

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
    }
}
