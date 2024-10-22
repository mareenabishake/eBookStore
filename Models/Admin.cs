using System;
using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models
{
    public class Admin
    {
        public int Id { get; set; }

        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "NIC No")]
        public string? NICNo { get; set; }

        public string? Address { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Contact No")]
        [Phone]
        public string? ContactNo { get; set; }

        public string? Role { get; set; }

        public string? UserId { get; set; }
    }
}
