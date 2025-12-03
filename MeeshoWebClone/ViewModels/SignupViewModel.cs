using System.ComponentModel.DataAnnotations;

namespace MeeshoWebClone.ViewModels
{
    public class SignupViewModel
    {
        [Required(ErrorMessage = "Please enter a phone number.")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^(\+91[-\s]?)?(0?91[-\s]?)?(0)?\d{10}$", ErrorMessage = "Please enter a valid phone number.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter an email address.")]
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; } = "User";
    }
}
