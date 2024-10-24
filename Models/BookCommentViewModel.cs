using System;
using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models
{
    // This class represents a view model for adding a comment to a book
    public class BookCommentViewModel
    {
        // ID of the book being commented on
        public int BookId { get; set; }

        // Title of the book being commented on
        public string BookTitle { get; set; }

        // The text content of the comment
        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string CommentText { get; set; }
    }
}
