using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models
{
    // This class represents an item in the shopping cart
    public class CartItem
    {
        // Unique identifier for the cart item
        public int Id { get; set; }

        // ID of the book associated with this cart item
        public int BookId { get; set; }

        // Title of the book
        public string Title { get; set; }

        // Price of the book
        public decimal Price { get; set; }

        // Quantity of this book in the cart
        public int Quantity { get; set; }

        // URL or path to the book's cover image
        public string ImageUrl { get; set; }
    }
}
