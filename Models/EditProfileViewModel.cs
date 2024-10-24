using System;
using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models
{
    // This class represents a view model for editing a user's profile
    public class EditProfileViewModel
    {
        // Unique identifier for the user
        public int Id { get; set; }

        // First name of the user
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        // Last name of the user
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        // National Identity Card number
        [Required]
        [Display(Name = "NIC Number")]
        public string NICNo { get; set; }

        // Physical address of the user
        [Required]
        public string Address { get; set; }

        // Date of birth of the user
        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        // Email address of the user
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Contact phone number of the user
        [Required]
        [Display(Name = "Contact Number")]
        public string ContactNo { get; set; }

        // Current password for verification
        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        // New password if the user wants to change it
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string? NewPassword { get; set; }

        // Confirmation of the new password
        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmNewPassword { get; set; }
    }
}
