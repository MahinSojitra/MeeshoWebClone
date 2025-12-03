using MeeshoWebClone.Enums;
using MeeshoWebClone.Models;
using Microsoft.AspNetCore.Identity;

namespace MeeshoWebClone.Areas.Admin.ViewModels
{
    public class UserDisplayViewModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string NormalizedUserName { get; set; }

        public string NormalizedEmail { get; set; }

        public string SecurityStamp { get; set; }

        public string ConcurrencyStamp { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public VerificationStatus Status { get; set; }

        public bool IsDeleted { get; set; }

        public string Role { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
    }
}
