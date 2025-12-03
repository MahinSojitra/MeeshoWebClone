using MeeshoWebClone.Areas.Admin.ViewModels;
using MeeshoWebClone.Data;
using MeeshoWebClone.Enums;
using MeeshoWebClone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeeshoWebClone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly MeeshoAppDbContext _context;

        public UserController(UserManager<User> userManager, MeeshoAppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserDisplayViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                // Skip Admin users
                if (roles.Contains("Admin"))
                {
                    continue;
                }

                // Fetch products if user is a Seller
                List<Product> products = new List<Product>();
                if (roles.Contains("Seller"))
                {
                    products = await _context.Products
                        .Where(p => p.SellerId == user.Id)
                        .ToListAsync();
                }

                var userViewModel = new UserDisplayViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber!,
                    NormalizedUserName = user.NormalizedUserName!,
                    NormalizedEmail = user.NormalizedEmail!,
                    SecurityStamp = user.SecurityStamp!,
                    ConcurrencyStamp = user.ConcurrencyStamp!,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    LockoutEnd = user.LockoutEnd,
                    LockoutEnabled = user.LockoutEnabled,
                    AccessFailedCount = user.AccessFailedCount,
                    Status = user.Status,
                    IsDeleted = user.IsDeleted,
                    Role = roles.FirstOrDefault()!,
                    Products = products
                };

                userViewModels.Add(userViewModel);
            }

            return View(userViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Approve(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Status = VerificationStatus.Approved;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Reject(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Status = VerificationStatus.Rejected;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userDisplayViewModel = new UserDisplayViewModel
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                NormalizedUserName = user.NormalizedUserName!,
                NormalizedEmail = user.NormalizedEmail!,
                SecurityStamp = user.SecurityStamp!,
                ConcurrencyStamp = user.ConcurrencyStamp!,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                LockoutEnd = user.LockoutEnd,
                LockoutEnabled = user.LockoutEnabled,
                AccessFailedCount = user.AccessFailedCount,
                Status = user.Status,
                IsDeleted = user.IsDeleted,
                Role = roles.FirstOrDefault()!
            };

            return View(userDisplayViewModel);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                user.IsDeleted = true;
                await _userManager.UpdateAsync(user);

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> UndoDelete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null && user.IsDeleted)
            {
                user.IsDeleted = false;
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("Index");
        }
    }
}
