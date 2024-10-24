using System;
using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models
{
    // This class represents an admin user in the eBookStore system
    public class Admin
    {
        // Unique identifier for the admin
        public int Id { get; set; }

        // First name of the admin
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        // Last name of the admin
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        // National Identity Card number
        [Display(Name = "NIC No")]
        public string? NICNo { get; set; }

        // Physical address of the admin
        public string? Address { get; set; }

        // Date of birth of the admin
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        // Email address of the admin
        [EmailAddress]
        public string? Email { get; set; }

        // Password of the admin (should be hashed in practice)
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        // Contact phone number of the admin
        [Display(Name = "Contact No")]
        [Phone]
        public string? ContactNo { get; set; }

        // Role of the admin (e.g., "Admin")
        public string? Role { get; set; }

        // Foreign key to link with AspNetUsers table
        public string? UserId { get; set; }
    }
}
