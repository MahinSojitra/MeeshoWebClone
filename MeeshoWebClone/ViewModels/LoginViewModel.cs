using System.ComponentModel.DataAnnotations;

namespace MeeshoWebClone.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter phone number.")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^(\+91[-\s]?)?(0?91[-\s]?)?(0)?\d{10}$", ErrorMessage = "Please enter a valid phone number.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter a email address.")]
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string? Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
