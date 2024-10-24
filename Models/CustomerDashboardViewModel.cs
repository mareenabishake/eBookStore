using System.Collections.Generic;

namespace eBookStore.Models
{
    // This class represents a view model for the customer dashboard
    public class CustomerDashboardViewModel
    {
        // Total number of orders placed by the customer
        public int TotalOrders { get; set; }

        // Total number of books purchased by the customer
        public int BooksPurchased { get; set; }

        // List of recent orders for quick access
        public List<Order> RecentOrders { get; set; }

        // List of recently added books to the store
        public List<Book> RecentBooks { get; set; }

        // Unique identifier for the customer
        public string CustomerId { get; set; }

        // Number of items currently in the customer's cart
        public int CartItemCount { get; set; }
    }
}
