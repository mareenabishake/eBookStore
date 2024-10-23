using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using eBookStore.Models;

namespace eBookStore.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; }
        public IdentityUser? Customer { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending";

        public string PaymentStatus { get; set; } = "Payment Due";

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
