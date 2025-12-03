using MeeshoWebClone.Enums;

namespace MeeshoWebClone.Areas.Admin.ViewModels
{
    public class VerificationPendingUserViewModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; init; }
        public VerificationStatus Status { get; set; }
    }
}
