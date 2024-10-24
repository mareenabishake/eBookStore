namespace eBookStore.Models
{
    // This class represents a view model for displaying purchased books
    public class PurchasedBookViewModel
    {
        // The book that was purchased
        public Book Book { get; set; }

        // Total quantity of this book purchased by the customer
        public int TotalQuantity { get; set; }
    }
}
