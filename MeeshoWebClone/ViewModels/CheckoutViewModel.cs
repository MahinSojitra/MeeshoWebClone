namespace MeeshoWebClone.ViewModels
{
    public class CheckoutViewModel
    {
        public List<CheckoutItemViewModel> Items { get; set; }
        public PaymentViewModel Payment { get; set; } = new();
    }

}
