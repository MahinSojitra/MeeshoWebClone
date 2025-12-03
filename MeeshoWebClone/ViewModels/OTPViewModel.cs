
using System.ComponentModel.DataAnnotations;

namespace MeeshoWebClone.ViewModels
{
    public class OTPViewModel
    {
        [Required(ErrorMessage = "Please enter otp.")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Please enter a valid otp of 4 digits.")]
        public int OTP { get; set; }
    }
}
