using System;
using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; }  // Changed to string to match IdentityUser's Id

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending";

        // You might want to add a collection of OrderItems here later
        public List<OrderItem> OrderItems { get; set; } // Make sure this is OrderItems, not CartItems
    }
}
