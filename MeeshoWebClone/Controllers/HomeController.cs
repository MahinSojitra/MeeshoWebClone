using MeeshoWebClone.Data;
using MeeshoWebClone.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace MeeshoWebClone.Controllers;

public class HomeController : Controller
{
    // Injecting the database context to access the data
    private readonly MeeshoAppDbContext _context;

    private readonly ILogger<HomeController> _logger;

    public HomeController(MeeshoAppDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Index()
    {
        var categories = _context.ProductCategories.ToList();

        var currentUserId = User.Identity!.IsAuthenticated ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!) : Guid.Empty;

        var likedProducts = currentUserId != Guid.Empty
            ? _context.UserLikedProducts
                .Where(ul => ul.UserId == currentUserId)
                .Select(ul => ul.ProductId)
                .ToHashSet()
            : new HashSet<Guid>();

        var cartItems = currentUserId != Guid.Empty
            ? _context.CartItems
                .Where(ci => ci.UserId == currentUserId)
                .ToDictionary(ci => ci.ProductId, ci => ci.Quantity)
            : new Dictionary<Guid, int>();

        var products = _context.Products
            .Include(p => p.Images)
            .Select(p => new ProductDisplayViewModel
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                DiscountedPrice = p.DiscountedPrice,
                DiscountPercentage = p.DiscountPercentage,
                FreeDelivery = p.FreeDelivery,
                StockQuantity = p.StockQuantity,
                Rating = p.Rating,
                ReviewCount = p.ReviewCount,
                CategoryName = p.Category.Name,
                SellerName = p.Seller.UserName!,
                SellerLocation = p.SellerLocation,
                FirstImageData = p.Images.First().ImageData,
                ImagesData = new List<byte[]>(),
                AvailableColors = _context.ProductColors.Select(c => c.ColorName).ToList(),
                AvailableSizes = _context.ProductSizes.Select(s => s.Size).ToList(),
                IsLikedByCurrentUser = likedProducts.Contains(p.ProductId),
                CartQuantity = cartItems.GetValueOrDefault(p.ProductId, 0)
            })
            .ToList();

        var model = new ProductPageViewModel
        {
            Categories = categories,
            Products = products
        };



        return View(model);
    }
}
