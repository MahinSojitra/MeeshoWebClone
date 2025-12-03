using MeeshoWebClone.Data;
using Microsoft.AspNetCore.Mvc;
using MeeshoWebClone.Models;
using MeeshoWebClone.Areas.Admin.ViewModels;
using MeeshoWebClone.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis;

namespace MeeshoWebClone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Seller")]
    public class ProductController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly MeeshoAppDbContext _context;

        public ProductController(MeeshoAppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var isSeller = User.IsInRole("Seller");

            IQueryable<Product> productsQuery = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Seller);

            if (isSeller)
            {
                productsQuery = productsQuery.Where(p => p.SellerId == user.Id);
            }

            var products = await productsQuery.Select(p => ConvertToViewModel(p, _context, false)).ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            // Get all sellers and admins
            var sellers = await _userManager.GetUsersInRoleAsync("Seller");
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var sellersAndAdmins = sellers.Concat(admins).Distinct();

            // Check if the current user is a seller
            var currentUser = await _userManager.GetUserAsync(User);
            bool isSeller = await _userManager.IsInRoleAsync(currentUser!, "Seller");

            var viewModel = new ProductCreateViewModel
            {
                AvailableColors = GetAvailableColors(),
                AvailableSizes = GetAvailableSizes(),
                AvailableCategories = GetAvailableCategories(),

                AvailableSellers = sellersAndAdmins.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.UserName
                }).ToList(),

                IsSeller = isSeller,
                SelectedSellerId = isSeller ? currentUser!.Id : Guid.Empty,
                SellerName = isSeller ? currentUser!.UserName ?? string.Empty : string.Empty
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isSeller = await _userManager.IsInRoleAsync(currentUser!, "Seller");

            // Validate Model
            if (!ValidateProductModel(model))
            {
                LoadDropdownData(model);
                return View(model);
            }

            try
            {
                var newProduct = new Product
                {
                    ProductId = Guid.NewGuid(),
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    DiscountedPrice = model.DiscountedPrice,
                    DiscountPercentage = model.DiscountPercentage,
                    StockQuantity = model.StockQuantity,
                    FreeDelivery = model.FreeDelivery,
                    SellerId = model.SelectedSellerId!,
                    SellerLocation = model.SellerLocation,
                    CategoryId = model.SelectedCategoryId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                _context.Products.Add(newProduct);

                await SaveProductImages(model.Images, newProduct.ProductId);
                SaveProductMappings(model.SelectedColorIds, model.SelectedSizeIds, newProduct.ProductId);

                // Save the product to the database
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the product.");
                Console.WriteLine(ex.Message);

                LoadDropdownData(model);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.ProductColorMappings)
                .Include(p => p.ProductSizeMappings)
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return NotFound();

            var viewModel = ConvertToViewModel(product, _context, true);

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            // Get all sellers and admins
            var sellers = await _userManager.GetUsersInRoleAsync("Seller");
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var sellersAndAdmins = sellers.Concat(admins).Distinct();

            var currentUser = await _userManager.GetUserAsync(User);
            bool isSeller = await _userManager.IsInRoleAsync(currentUser!, "Seller");
            bool isAdmin = await _userManager.IsInRoleAsync(currentUser!, "Admin");

            // Find the product
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.ProductColorMappings)
                .Include(p => p.ProductSizeMappings)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return NotFound();

            if (isSeller && product.SellerId != currentUser!.Id)
            {
                return Forbid();
            }

            var model = new ProductEditViewModel
            {
                Id = product.ProductId,
                Name = product.Name,
                SelectedCategoryId = product.CategoryId,
                Description = product.Description,
                Price = product.Price,
                DiscountedPrice = product.DiscountedPrice,
                DiscountPercentage = product.DiscountPercentage,
                StockQuantity = product.StockQuantity,
                FreeDelivery = product.FreeDelivery,
                SellerName = product.Seller.UserName,
                SellerLocation = product.SellerLocation,
                AvailableCategories = await _context.ProductCategories.ToListAsync(),
                AvailableColors = await _context.ProductColors.ToListAsync(),
                AvailableSizes = await _context.ProductSizes.ToListAsync(),
                SelectedColorIds = product.ProductColorMappings.Select(c => c.ColorId).ToList(),
                SelectedSizeIds = product.ProductSizeMappings.Select(s => s.SizeId).ToList(),
                ExistingImageIds = product.Images.Select(img => img.ImageId).ToList(),
                ExistingImages = product.Images.Select(img => img.ImageData).ToList(),
                RowVersion = product.RowVersion,
                IsSeller = isSeller,
                SelectedSellerId = isSeller ? currentUser!.Id : product.SellerId,
                AvailableSellers = sellersAndAdmins.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.UserName
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid Id, ProductEditViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var sellers = await _userManager.GetUsersInRoleAsync("Seller");
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var sellersAndAdmins = sellers.Concat(admins).Distinct();

            bool isSeller = await _userManager.IsInRoleAsync(currentUser!, "Seller");
            bool isAdmin = await _userManager.IsInRoleAsync(currentUser!, "Admin");

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductColorMappings)
                .Include(p => p.ProductSizeMappings)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == Id);

            if (product == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await PopulateViewModel(model, product, currentUser!, isSeller, sellersAndAdmins);
                return View(model);
            }

            // Set concurrency token
            var dbVersion = _context.Entry(product).OriginalValues["RowVersion"] = model.RowVersion;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update basic fields
                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.DiscountedPrice = model.DiscountedPrice;
                product.DiscountPercentage = model.DiscountPercentage;
                product.StockQuantity = model.StockQuantity;
                product.FreeDelivery = model.FreeDelivery;
                product.SellerId = model.SelectedSellerId;
                product.SellerLocation = model.SellerLocation;
                product.CategoryId = model.SelectedCategoryId;
                product.UpdatedAt = DateTime.Now;

                // Remove selected images
                if (model.ImageIdsToRemove != null)
                {
                    var imagesToRemove = product.Images
                        .Where(img => model.ImageIdsToRemove.Contains(img.ImageId))
                        .ToList();

                    _context.ProductImages.RemoveRange(imagesToRemove);
                }

                // Update color mappings
                product.ProductColorMappings.Clear();
                if (model.SelectedColorIds != null)
                {
                    foreach (var colorId in model.SelectedColorIds)
                    {
                        await _context.ProductColorMappings.AddAsync(new ProductColorMapping
                        {
                            ProductId = product.ProductId,
                            ColorId = colorId
                        });
                    }
                }

                // Update size mappings
                product.ProductSizeMappings.Clear();
                if (model.SelectedSizeIds != null)
                {
                    foreach (var sizeId in model.SelectedSizeIds)
                    {
                        await _context.ProductSizeMappings.AddAsync(new ProductSizeMapping
                        {
                            ProductId = product.ProductId,
                            SizeId = sizeId
                        });
                    }
                }

                // Add new images
                if (model.NewImages != null && model.NewImages.Any())
                {
                    foreach (var file in model.NewImages.Where(f => f.Length > 0))
                    {
                        using var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream);

                        var productImage = new ProductImage
                        {
                            ProductId = product.ProductId,
                            ImageData = memoryStream.ToArray()
                        };
                        await _context.ProductImages.AddAsync(productImage);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();

                ModelState.AddModelError("", "Conflict. Product has been updated since you opened it.");
                await PopulateViewModel(model, product, currentUser!, isSeller, sellersAndAdmins);
                return View(model);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                await PopulateViewModel(model, product, currentUser!, isSeller, sellersAndAdmins);
                return View(model);
            }
        }

        private async Task PopulateViewModel(ProductEditViewModel model, Product product, User currentUser, bool isSeller, IEnumerable<User> sellersAndAdmins)
        {
            model.AvailableCategories = await _context.ProductCategories.ToListAsync();
            model.AvailableColors = await _context.ProductColors.ToListAsync();
            model.AvailableSizes = await _context.ProductSizes.ToListAsync();
            model.ExistingImageIds = product.Images.Select(img => img.ImageId).ToList();
            model.ExistingImages = product.Images.Select(img => img.ImageData).ToList();
            model.RowVersion = product.RowVersion;
            model.IsSeller = isSeller;
            model.AvailableSellers = sellersAndAdmins.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.UserName
            }).ToList();
            model.SelectedSellerId = isSeller ? currentUser.Id : product.SellerId;
            model.SellerName = isSeller ? currentUser.UserName ?? string.Empty : string.Empty;
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isSeller = await _userManager.IsInRoleAsync(currentUser!, "Seller");

            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            if (isSeller && product.SellerId != currentUser!.Id)
            {
                return Forbid();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private bool ValidateProductModel(ProductCreateViewModel model)
        {
            if (model.SelectedColorIds == null || !model.SelectedColorIds.Any())
                ModelState.AddModelError("SelectedColorIds", "Please select at least one color.");

            if (model.SelectedSizeIds == null || !model.SelectedSizeIds.Any())
                ModelState.AddModelError("SelectedSizeIds", "Please select at least one size.");

            if (model.Images == null || !model.Images.Any())
                ModelState.AddModelError("Images", "Please upload at least one product image.");

            return ModelState.IsValid;
        }

        private void LoadDropdownData(ProductCreateViewModel model)
        {
            model.AvailableCategories = GetAvailableCategories();
            model.AvailableColors = GetAvailableColors();
            model.AvailableSizes = GetAvailableSizes();
        }

        private async Task SaveProductImages(IEnumerable<IFormFile>? images, Guid productId)
        {
            if (images == null || !images.Any())
                return;

            foreach (var file in images.Where(f => f.Length > 0))
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var productImage = new ProductImage
                {
                    ProductId = productId,
                    ImageData = memoryStream.ToArray()
                };

                await _context.ProductImages.AddAsync(productImage);
                await _context.SaveChangesAsync();
            }
        }

        private void SaveProductMappings(List<Guid>? colorIds, List<Guid>? sizeIds, Guid productId)
        {
            if (colorIds != null && colorIds.Any())
            {
                var productColors = colorIds.Select(colorId => new ProductColorMapping
                {
                    ProductId = productId,
                    ColorId = colorId
                }).ToList();
                _context.ProductColorMappings.AddRange(productColors);
            }

            if (sizeIds != null && sizeIds.Any())
            {
                var productSizes = sizeIds.Select(sizeId => new ProductSizeMapping
                {
                    ProductId = productId,
                    SizeId = sizeId
                }).ToList();
                _context.ProductSizeMappings.AddRange(productSizes);
            }

            _context.SaveChanges();
        }

        private static ProductDisplayViewModel ConvertToViewModel(Product p, MeeshoAppDbContext context, bool includeImages = false)
        {
            // Get color IDs from ProductColorMappings
            var colorIds = p.ProductColorMappings.Select(m => m.ColorId).ToList();

            // Get actual color names based on those IDs
            var availableColors = context.ProductColors
                .Where(c => colorIds.Contains(c.ColorId))
                .Select(c => c.ColorName)
                .ToList();

            // Get size IDs from ProductSizeMappings
            var sizeIds = p.ProductSizeMappings.Select(m => m.SizeId).ToList();

            // Get actual size values based on those IDs
            var availableSizes = context.ProductSizes
                .Where(s => sizeIds.Contains(s.SizeId))
                .Select(s => s.Size)
                .ToList();

            return new ProductDisplayViewModel
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                DiscountedPrice = p.DiscountedPrice,
                DiscountPercentage = p.DiscountPercentage,
                StockQuantity = p.StockQuantity,
                FreeDelivery = p.FreeDelivery,
                SellerName = p.Seller?.UserName!,
                SellerLocation = p.SellerLocation,
                Rating = p.Rating,
                ReviewCount = p.ReviewCount,
                CategoryName = p.Category?.Name!,
                FirstImageData = p.Images.FirstOrDefault()?.ImageData!,
                ImagesData = includeImages ? p.Images.Select(img => img.ImageData).ToList() : null,
                AvailableColors = availableColors,
                AvailableSizes = availableSizes,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
        }

        private List<ProductCategory> GetAvailableCategories() => _context.ProductCategories.ToList();
        private List<ProductColor> GetAvailableColors() => _context.ProductColors.ToList();
        private List<ProductSize> GetAvailableSizes() => _context.ProductSizes.ToList();
    }
}