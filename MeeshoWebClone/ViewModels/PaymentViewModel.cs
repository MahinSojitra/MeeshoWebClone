using System.ComponentModel.DataAnnotations;

namespace MeeshoWebClone.ViewModels
{
    public class PaymentViewModel
    {
        [Display(Name = "Payment Method")]
        [Required(ErrorMessage = "Please select a payment method.")]
        public string SelectedMethod { get; set; } = "cod";

        [Display(Name = "Card Holder Name")]
        [Required(ErrorMessage = "Card Holder Name is required.")]
        public string CardHolderName { get; set; }

        [Display(Name = "Card Number")]
        [Required(ErrorMessage = "Card Number is required.")]
        [CreditCard(ErrorMessage = "Invalid card number.")]
        public string CardNumber { get; set; }

        [Display(Name = "Expiry Date (MM/YY)")]
        [Required(ErrorMessage = "Expiry Date is required.")]
        public string ExpiryDate { get; set; }

        [Display(Name = "CVV")]
        [Required(ErrorMessage = "CVV is required.")]
        [StringLength(4, MinimumLength = 3, ErrorMessage = "CVV must be 3 or 4 digits.")]
        public string CVV { get; set; }

        [Display(Name = "UPI ID")]
        [Required(ErrorMessage = "UPI ID is required.")]
        public string UPIId { get; set; }

        [Display(Name = "Bank Name")]
        public string BankName { get; set; }
    }
}
