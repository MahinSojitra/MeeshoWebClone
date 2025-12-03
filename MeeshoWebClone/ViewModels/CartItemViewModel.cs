namespace MeeshoWebClone.ViewModels
{
    public class CartItemViewModel
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int DiscountPercentage { get; set; }
        public int Quantity { get; set; }
        public byte[] FirstImage { get; set; }
        public DateTime AddedAt { get; set; }
    }

}
