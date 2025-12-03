using Microsoft.AspNetCore.Identity;
using MeeshoWebClone.Enums;
using System.Collections.Generic;

namespace MeeshoWebClone.Models
{
    public class User : IdentityUser<Guid>
    {
        public VerificationStatus Status { get; set; } = VerificationStatus.Pending;
        public bool IsDeleted { get; set; } = false;

        public virtual List<Product> Products { get; set; } = new List<Product>();
        public virtual List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual List<UserLikedProduct> LikedProducts { get; set; } = new List<UserLikedProduct>();
    }
}
