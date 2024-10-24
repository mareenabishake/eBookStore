using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models
{
    // This class represents a view model for the login form
    public class LoginViewModel
    {
        // User's email address for login
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // User's password for login
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Option to remember the user's login
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
