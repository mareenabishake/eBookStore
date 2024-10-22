using System;
using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models
{
    public class BookCommentViewModel
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string CommentText { get; set; }
    }
}
