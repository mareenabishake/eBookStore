using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using eBookStore.Models;

namespace eBookStore.Models
{
    // This class represents an order in the eBookStore system
    public class Order
    {
        // Unique identifier for the order
        public int Id { get; set; }

        // Foreign key to link with the Customer
        [Required]
        public string CustomerId { get; set; }

        // Navigation property to the associated Customer
        public IdentityUser? Customer { get; set; }

        // Date and time when the order was placed
        [Required]
        public DateTime OrderDate { get; set; }

        // Total amount of the order
        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        // Current status of the order (e.g., "Pending", "Shipped", "Delivered")
        public string Status { get; set; } = "Pending";

        // Current payment status of the order (e.g., "Payment Due", "Paid")
        public string PaymentStatus { get; set; } = "Payment Due";

        // Collection of order items associated with this order
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
