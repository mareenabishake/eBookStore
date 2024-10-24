using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace eBookStore.Models
{
    public class BookComment
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(500)]
        public string CommentText { get; set; }

        public DateTime CommentDate { get; set; }

        public Book Book { get; set; }

        public IdentityUser? User { get; set; }
    }
}
