
namespace MovieStore.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        // Foreign Key reference to User
        public int UserId { get; set; }
        public User User { get; set; } // Navigation property

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }

        // Movie-specific details (Foreign Key to Movie)
        public int MovieId { get; set; }
        public Movie Movie { get; set; } // Navigation property

        // Price of the movie at the time of purchase
        public decimal Price { get; set; }

        public int Rating { get; set; }
        public override string ToString()
        {
            return $"OrderId: {OrderId}, UserId: {UserId}, OrderDate: {OrderDate}, TotalAmount: {TotalAmount}, MovieId: {MovieId}, Price: {Price}";
        }
    }

}
