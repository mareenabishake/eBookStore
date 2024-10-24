using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace eBookStore.Models
{
    // This class represents a comment on a book
    public class BookComment
    {
        // Unique identifier for the comment
        public int Id { get; set; }

        // Foreign key to link with the Book
        [Required]
        public int BookId { get; set; }

        // Foreign key to link with the User who made the comment
        [Required]
        public string UserId { get; set; }

        // The text content of the comment
        [Required]
        [StringLength(500)]
        public string CommentText { get; set; }

        // Date and time when the comment was posted
        public DateTime CommentDate { get; set; }

        // Navigation property to the associated Book
        public Book Book { get; set; }

        // Navigation property to the associated User
        // Nullable to handle cases where the user might be deleted
        public IdentityUser? User { get; set; }
    }
}
