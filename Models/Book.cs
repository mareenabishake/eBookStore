using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace eBookStore.Models
{
    // This class represents a book in the eBookStore system
    public class Book
    {
        // Unique identifier for the book
        public int Id { get; set; }

        // Title of the book
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        // Author of the book
        [Required]
        [StringLength(100)]
        public string Author { get; set; }

        // International Standard Book Number
        [Required]
        [StringLength(13, MinimumLength = 10)]
        [Display(Name = "ISBN")]
        public string ISBN { get; set; }

        // Detailed description of the book
        [StringLength(500)]
        public string Description { get; set; }

        // Price of the book
        [Required]
        [Range(0, 1000)]
        public decimal Price { get; set; }

        // Number of copies available in stock
        public int StockQuantity { get; set; }

        // URL or path to the book's cover image
        // Nullable to allow books without images
        public string? ImageUrl { get; set; }

        // Publisher of the book
        [Required]
        [StringLength(100)]
        public string Publisher { get; set; }

        // Genre or category of the book
        [Required]
        [StringLength(50)]
        public string Genre { get; set; }

        // Year the book was published
        [Required]
        [Range(1000, 9999)]
        public int PublishedYear { get; set; }

        // Collection of comments associated with this book
        // Allows for easy access to all comments for a book
        public ICollection<BookComment> BookComments { get; set; }
    }
}
