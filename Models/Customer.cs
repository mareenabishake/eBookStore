using System;
using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models
{
    // This class represents a customer in the eBookStore system
    public class Customer
    {
        // Unique identifier for the customer
        public int Id { get; set; }

        // First name of the customer
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        // Last name of the customer
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        // National Identity Card number
        [Required]
        [Display(Name = "NIC No")]
        public string NICNo { get; set; }

        // Physical address of the customer
        [Required]
        public string Address { get; set; }

        // Date of birth of the customer
        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        // Email address of the customer
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Contact phone number of the customer
        [Required]
        [Display(Name = "Contact No")]
        [Phone]
        public string ContactNo { get; set; }

        // Password of the customer (should be hashed in practice)
        // Nullable to allow for external authentication methods
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        // Role of the user (e.g., "Customer")
        public string Role { get; set; } = "Customer";

        // Foreign key to link with AspNetUsers table
        public string? UserId { get; set; }
    }
}
