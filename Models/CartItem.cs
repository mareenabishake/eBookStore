using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
    }
}