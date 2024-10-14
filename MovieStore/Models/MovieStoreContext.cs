using Microsoft.EntityFrameworkCore;

namespace MovieStore.Models
{
    public class MovieStoreContext : DbContext
    {
        public MovieStoreContext(DbContextOptions<MovieStoreContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }

    
    }
}
