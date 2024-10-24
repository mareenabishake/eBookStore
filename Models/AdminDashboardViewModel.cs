using System.Collections.Generic;

namespace eBookStore.Models
{
    // This class represents a view model for the admin dashboard
    public class AdminDashboardViewModel
    {
        // Total number of books in the store
        public int TotalBooks { get; set; }

        // Total number of registered customers
        public int TotalCustomers { get; set; }

        // Total number of orders placed
        public int TotalOrders { get; set; }

        // List of recently added books
        public List<Book> RecentBooks { get; set; }

        // List of recent orders
        public List<Order> RecentOrders { get; set; }
    }
}
