using MeeshoWebClone.Areas.Admin.ViewModels;
using MeeshoWebClone.Enums;
using MeeshoWebClone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeeshoWebClone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Seller")]
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public HomeController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var verificationPendingUsers = await _userManager.Users
                        .Where(u => u.Status == VerificationStatus.Pending)
                        .Select(u => new VerificationPendingUserViewModel
                        {
                            Id = u.Id,
                            UserName = u.UserName!,
                            Email = u.Email!,
                            PhoneNumber = u.PhoneNumber!,
                            Status = u.Status
                        })
                        .ToListAsync();

            return View(new AdminDashboardViewModel
            {
                VerificationPendingUsers = verificationPendingUsers
            });
        }
    }
}
