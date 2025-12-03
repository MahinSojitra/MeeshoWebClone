using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeeshoWebClone.Models
{
    public class ProductColor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ColorId { get; set; } = Guid.NewGuid();

        [Required]
        public string ColorName { get; set; }

        [Required]
        public string ColorHex { get; set; }

        public List<ProductColorMapping> ProductColorMappings { get; set; } = new List<ProductColorMapping>();
    }
}
