using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeeshoWebClone.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ProductId { get; set; } = Guid.NewGuid();

        [Required, MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Precision(18, 2)]
        public decimal Price { get; set; }

        [Precision(18, 2)]
        public decimal DiscountedPrice { get; set; }
        public int DiscountPercentage { get; set; }

        public bool FreeDelivery { get; set; }
        public int StockQuantity { get; set; }

        public double Rating { get; set; }
        public int ReviewCount { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public ProductCategory Category { get; set; }

        [Required]
        public Guid SellerId { get; set; }

        [ForeignKey("SellerId")]
        public User Seller { get; set; }

        public string SellerLocation { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual List<ProductImage> Images { get; set; } = new List<ProductImage>();
        public virtual List<ProductColorMapping> ProductColorMappings { get; set; } = new List<ProductColorMapping>();
        public virtual List<ProductSizeMapping> ProductSizeMappings { get; set; } = new List<ProductSizeMapping>();
        public virtual List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual List<UserLikedProduct> LikedByUsers { get; set; } = new List<UserLikedProduct>();

        [Timestamp]
        public byte[] RowVersion { get; set; } = default!;
    }
}
