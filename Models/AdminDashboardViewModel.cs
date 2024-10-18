using System.Collections.Generic;

namespace eBookStore.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalOrders { get; set; }
        public List<Book> RecentBooks { get; set; }
        public List<Order> RecentOrders { get; set; }
    }
}
