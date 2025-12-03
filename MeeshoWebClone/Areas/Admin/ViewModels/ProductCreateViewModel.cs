using System.ComponentModel.DataAnnotations;
using MeeshoWebClone.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MeeshoWebClone.Areas.Admin.ViewModels
{
    public class ProductCreateViewModel
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(255, ErrorMessage = "Product name cannot exceed 255 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Product description is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Product price is required.")]
        [Range(1, 1000000, ErrorMessage = "Price must be between 1 and 1,000,000.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Discounted price is required.")]
        [Range(0, 1000000, ErrorMessage = "Discounted price must be between 0 and 1,000,000.")]
        public decimal DiscountedPrice { get; set; }

        [Required(ErrorMessage = "Discount percentage is required.")]
        [Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100.")]
        public int DiscountPercentage { get; set; }

        [Required(ErrorMessage = "Stock quantity is required.")]
        [Range(1, 10000, ErrorMessage = "Stock quantity must be between 1 and 10,000.")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Category selection is required.")]
        public Guid SelectedCategoryId { get; set; }

        [Required(ErrorMessage = "Seller name is required.")]
        [StringLength(100, ErrorMessage = "Seller name cannot exceed 100 characters.")]
        public string SellerName { get; set; }

        [Required(ErrorMessage = "Seller location is required.")]
        [StringLength(100, ErrorMessage = "Seller location cannot exceed 100 characters.")]
        public string SellerLocation { get; set; }

        public bool FreeDelivery { get; set; } = false;

        [Required(ErrorMessage = "Please select product images.")]
        [MinLength(1, ErrorMessage = "At least one product image is required.")]
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();

        [MinLength(1, ErrorMessage = "Please select at least one color.")]
        public List<Guid> SelectedColorIds { get; set; } = new List<Guid>();

        [MinLength(1, ErrorMessage = "Please select at least one size.")]
        public List<Guid> SelectedSizeIds { get; set; } = new List<Guid>();

        public List<ProductColor> AvailableColors { get; set; } = new List<ProductColor>();
        public List<ProductSize> AvailableSizes { get; set; } = new List<ProductSize>();
        public List<ProductCategory> AvailableCategories { get; set; } = new List<ProductCategory>();

        [Required(ErrorMessage = "Please select seller.")]
        public Guid SelectedSellerId { get; set; }

        public bool IsSeller { get; set; }

        public List<SelectListItem> AvailableSellers { get; set; } = new List<SelectListItem>();
    }
}
