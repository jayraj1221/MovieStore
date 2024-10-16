using Microsoft.EntityFrameworkCore;
using MovieStore.Models;

namespace MovieStore.Repository
{
    public interface IUser
    {
        Task<User> RegisterAsync(User user);
        Task<User> LoginAsync(string username, string password);
        User GetUserByIdAsync(int id);
        public void AddOrder(Order order);

        public User GetUserByUsername(string userName);

        public bool HasUserPurchasedMovie(int userId, int movieId);

        IEnumerable<Order> GetOrdersByUserId(int userId);

        public void UpdateOrder(Order order);

        public Order GetOrderById(int orderId);

        public void UpdateUser(User user);

    }

}

