using MediatR;

namespace MeeshoWebClone.Commands
{
    public class UpdateCartByProductIdAndChangeCommand : IRequest<int>
    {
        public Guid ProductId { get; set; }
        public int Change { get; set; }
        public Guid UserId { get; set; }
    }
}
