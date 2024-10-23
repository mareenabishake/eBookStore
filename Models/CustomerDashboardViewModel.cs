using System.Collections.Generic;

namespace eBookStore.Models
{
    public class CustomerDashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int BooksPurchased { get; set; }
        public List<Order> RecentOrders { get; set; }
        public List<Book> RecentBooks { get; set; }
        public string CustomerId { get; set; }
        public int CartItemCount { get; set; }
    }
}
