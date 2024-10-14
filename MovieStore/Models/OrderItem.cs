namespace MovieStore.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }

        // Foreign Key reference to Order
        public int OrderId { get; set; }
        public Order Order { get; set; } // Navigation property

        // Foreign Key reference to Movie
        public int MovieId { get; set; }
        public Movie Movie { get; set; } // Navigation property

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
