using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace eBookStore.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Author { get; set; }

        [Required]
        [StringLength(13, MinimumLength = 10)]
        [Display(Name = "ISBN")]
        public string ISBN { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0, 1000)]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        [StringLength(100)]
        public string Publisher { get; set; }

        [Required]
        [StringLength(50)]
        public string Genre { get; set; }

        [Required]
        [Range(1000, 9999)]
        public int PublishedYear { get; set; }

        public ICollection<BookComment> BookComments { get; set; }
    }
}
