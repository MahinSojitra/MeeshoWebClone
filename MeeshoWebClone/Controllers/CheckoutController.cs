using MeeshoWebClone.Data;
using MeeshoWebClone.Models;
using MeeshoWebClone.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MeeshoWebClone.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly MeeshoAppDbContext _context;
        private readonly UserManager<User> _userManager;

        public CheckoutController(MeeshoAppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                    .ThenInclude(p => p.Images)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            var checkoutItems = cartItems.Any()
                ? cartItems
                    .Where(c => c.Product != null)
                    .Select(c => new CheckoutItemViewModel
                    {
                        ProductId = c.ProductId,
                        Name = c.Product!.Name,
                        Description = c.Product.Description!,
                        Price = c.Product.Price,
                        DiscountPercentage = c.Product.DiscountPercentage,
                        DiscountedPrice = c.Product.DiscountedPrice,
                        StockQuantity = c.Product.StockQuantity,
                        FirstImage = c.Product.Images.FirstOrDefault()?.ImageData ?? new byte[0],
                        Quantity = c.Quantity,
                        AddedAt = c.AddedAt
                    })
                    .ToList()
                : new();

            var viewModel = new CheckoutViewModel
            {
                Items = checkoutItems,
                Payment = new()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult OrderConfirmation(CheckoutViewModel model)
        {
            return View();
        }
    }
}
