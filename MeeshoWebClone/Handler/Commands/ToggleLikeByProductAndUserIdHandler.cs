using MediatR;
using MeeshoWebClone.Commands;
using MeeshoWebClone.Data;
using MeeshoWebClone.Models;
using Microsoft.EntityFrameworkCore;

namespace MeeshoWebClone.Handler.Commands
{
    public class ToggleLikeByProductAndUserIdHandler : IRequestHandler<ToggleLikeByProductAndUserIdCommand, bool>
    {
        private readonly MeeshoAppDbContext _context;
        
        public ToggleLikeByProductAndUserIdHandler(MeeshoAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ToggleLikeByProductAndUserIdCommand request, CancellationToken cancellationToken)
        {
            var existingLike = await _context.UserLikedProducts
                .FirstOrDefaultAsync(l => l.UserId == request.UserId && l.ProductId == request.ProductId, cancellationToken);
            
            if (existingLike != null)
            {
                _context.UserLikedProducts.Remove(existingLike);
            }
            else
            {
                _context.UserLikedProducts.Add(new UserLikedProduct
                {
                    UserId = request.UserId,
                    ProductId = request.ProductId,
                    LikedAt = DateTime.UtcNow
                });
            }
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
