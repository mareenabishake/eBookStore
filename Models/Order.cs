using System;
using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending";

        // You might want to add a collection of OrderItems here later
        // public ICollection<OrderItem> OrderItems { get; set; }
    }
}

