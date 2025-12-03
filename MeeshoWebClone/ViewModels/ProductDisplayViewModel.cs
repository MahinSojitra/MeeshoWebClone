namespace MeeshoWebClone.ViewModels
{
    public class ProductDisplayViewModel
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int DiscountPercentage { get; set; }
        public bool FreeDelivery { get; set; }
        public int StockQuantity { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public string CategoryName { get; set; }    
        public string SellerId { get; set; }
        public string SellerName { get; set; }
        public string SellerLocation { get; set; }
        public byte[] FirstImageData { get; set; }
        public List<byte[]>? ImagesData { get; set; } = new List<byte[]>();
        public List<string> AvailableColors { get; set; } = new List<string>();
        public List<string> AvailableSizes { get; set; } = new List<string>();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; }

        public bool IsLikedByCurrentUser { get; set; }
        public int CartQuantity { get; set; }
    }
}
