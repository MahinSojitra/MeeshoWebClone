using MediatR;
using MeeshoWebClone.Commands;
using MeeshoWebClone.Data;
using MeeshoWebClone.Models;
using Microsoft.EntityFrameworkCore;

namespace MeeshoWebClone.Handler.Commands
{
    public class UpdateCartByProductIdAndChangeHandler : IRequestHandler<UpdateCartByProductIdAndChangeCommand, int>
    {
        private readonly MeeshoAppDbContext _context;

        public UpdateCartByProductIdAndChangeHandler(MeeshoAppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(UpdateCartByProductIdAndChangeCommand request, CancellationToken cancellationToken)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == request.UserId && c.ProductId == request.ProductId, cancellationToken);
            if (cartItem != null)
            {
                cartItem.Quantity += request.Change;
                if (cartItem.Quantity <= 0)
                {
                    _context.CartItems.Remove(cartItem);
                }
            }
            else if (request.Change > 0)
            {
                _context.CartItems.Add(new CartItem
                {
                    UserId = request.UserId,
                    ProductId = request.ProductId,
                    Quantity = 1
                });
            }
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
