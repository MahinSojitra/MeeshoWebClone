using System.ComponentModel.DataAnnotations;

namespace MeeshoWebClone.Areas.Admin.ViewModels
{
    public class ProductCategoryCreateViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        [Display(Name = "Category Name")]
        public string Name { get; set; }
    }
}
