using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeeshoWebClone.Models
{
    public class ProductImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ImageId { get; set; } = Guid.NewGuid();

        [Required]
        public byte[] ImageData { get; set; }

        public Guid ProductId { get; set; }

        public Product Product { get; set; }
    }
}
