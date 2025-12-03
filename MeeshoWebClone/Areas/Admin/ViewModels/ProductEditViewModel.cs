using System.ComponentModel.DataAnnotations;
using MeeshoWebClone.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MeeshoWebClone.Areas.Admin.ViewModels
{
    public class ProductEditViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(255, ErrorMessage = "Product name cannot exceed 255 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Category selection is required.")]
        public Guid SelectedCategoryId { get; set; }
        public List<ProductCategory> AvailableCategories { get; set; } = new();

        [Required(ErrorMessage = "Product description is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(1, 1000000, ErrorMessage = "Price must be between 1 and 1,000,000.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Discounted price is required.")]
        [Range(1, 1000000, ErrorMessage = "Price must be between 1 and 1,000,000.")]
        public decimal DiscountedPrice { get; set; }

        [Required(ErrorMessage = "Discount percentage is required.")]
        [Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100.")]
        public int DiscountPercentage { get; set; }

        [Required(ErrorMessage = "Stock quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Stock quantity must be at least 1.")]
        public int StockQuantity { get; set; }

        [StringLength(100, ErrorMessage = "Seller name cannot exceed 100 characters.")]
        public string? SellerName { get; set; }

        [Required(ErrorMessage = "Seller location is required.")]
        [StringLength(200, ErrorMessage = "Seller location cannot exceed 200 characters.")]
        public string SellerLocation { get; set; }

        public bool FreeDelivery { get; set; }

        [MinLength(1, ErrorMessage = "Please select at least one color.")]
        public List<Guid> SelectedColorIds { get; set; } = new();
        public List<ProductColor> AvailableColors { get; set; } = new();

        [MinLength(1, ErrorMessage = "Please select at least one size.")]
        public List<Guid> SelectedSizeIds { get; set; } = new();
        public List<ProductSize> AvailableSizes { get; set; } = new();

        public List<byte[]> ExistingImages { get; set; } = new();

        public List<Guid> ExistingImageIds { get; set; } = new();

        public List<Guid>? ImageIdsToRemove { get; set; } = new();

        [Required(ErrorMessage = "Please select product images.")]
        [MinLength(1, ErrorMessage = "Please select at least one image.")]
        public List<IFormFile>? NewImages { get; set; } = new();

        public byte[] RowVersion { get; set; }

        [Required(ErrorMessage = "Please select seller.")]
        public Guid SelectedSellerId { get; set; }

        public bool IsSeller { get; set; }

        public List<SelectListItem> AvailableSellers { get; set; } = new List<SelectListItem>();
    }
}
