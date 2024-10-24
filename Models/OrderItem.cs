using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models
{
    // This class represents an individual item within an order
    public class OrderItem
    {
        // Unique identifier for the order item
        [Key]
        public int Id { get; set; }

        // Foreign key to link with the Order
        public int OrderId { get; set; }

        // Navigation property to the associated Order
        public Order Order { get; set; }

        // Foreign key to link with the Book
        public int BookId { get; set; }

        // Navigation property to the associated Book
        public Book Book { get; set; }

        // Quantity of this book in the order
        public int Quantity { get; set; }

        // Price of the book at the time of order
        public decimal Price { get; set; }
    }
}
