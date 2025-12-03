using MediatR;
using MeeshoWebClone.Data;
using MeeshoWebClone.Queries;
using MeeshoWebClone.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MeeshoWebClone.Handler.Queries
{
    public class GetCartItemListHandler : IRequestHandler<GetCartItemListQuery, List<CartItemViewModel>>
    {
        private readonly MeeshoAppDbContext _context;
        
        public GetCartItemListHandler(MeeshoAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CartItemViewModel>> Handle(GetCartItemListQuery request, CancellationToken cancellationToken)
        {
            var cartItems = await _context.CartItems
                .Where(c => c.UserId == request.UserId)
                .Select(c => new CartItemViewModel
                {
                    ProductId = c.ProductId,
                    Name = c.Product.Name,
                    Description = c.Product.Description,
                    Price = c.Product.Price,
                    DiscountedPrice = c.Product.DiscountedPrice,
                    DiscountPercentage = c.Product.DiscountPercentage,
                    StockQuantity = c.Product.StockQuantity,
                    Quantity = c.Quantity,
                    FirstImage = c.Product.Images.FirstOrDefault()!.ImageData,
                    AddedAt = c.AddedAt
                })
                .ToListAsync(cancellationToken);
            return cartItems;
        }
    }
}
