using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeeshoWebClone.Models
{
    public class ProductSize
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SizeId { get; set; } = Guid.NewGuid();

        [Required]
        public string Size { get; set; }

        public List<ProductSizeMapping> ProductSizeMappings { get; set; } = new List<ProductSizeMapping>();
    }
}
