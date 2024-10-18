using System;
using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models
{
    public class Admin
    {
        public int Id { get; set; }
        
        public string Username { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "NIC No")]
        public string NICNo { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Contact No")]
        [Phone]
        public string ContactNo { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Role { get; set; } = "Admin";
    }
}
