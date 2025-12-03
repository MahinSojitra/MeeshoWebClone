using MediatR;
using MeeshoWebClone.Data;
using MeeshoWebClone.Queries;
using MeeshoWebClone.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MeeshoWebClone.Handler.Queries
{
    public class GetLikedProductListHandler : IRequestHandler<GetLikedProductListQuery, List<LikedProductViewModel>>
    {
        private readonly MeeshoAppDbContext _context;
        
        public GetLikedProductListHandler(MeeshoAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<LikedProductViewModel>> Handle(GetLikedProductListQuery request, CancellationToken cancellationToken)
        {
            var likedProducts = await _context.UserLikedProducts
                .Where(p => p.UserId == request.UserId)
                .Select(p => new LikedProductViewModel
                {
                    ProductId = p.ProductId,
                    Name = p.Product.Name,
                    Description = p.Product.Description,
                    Price = p.Product.Price,
                    DiscountedPrice = p.Product.DiscountedPrice,
                    DiscountPercentage = p.Product.DiscountPercentage,
                    StockQuantity = p.Product.StockQuantity,
                    FirstImage = p.Product.Images.FirstOrDefault()!.ImageData,
                    IsInCart = _context.CartItems.Any(c => c.UserId == request.UserId && c.ProductId == p.ProductId),
                    LikedAt = p.LikedAt
                })
                .ToListAsync(cancellationToken);
            return likedProducts;
        }
    }
}
