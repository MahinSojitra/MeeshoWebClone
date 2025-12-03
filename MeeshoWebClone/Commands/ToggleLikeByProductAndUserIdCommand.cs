using MediatR;

namespace MeeshoWebClone.Commands
{
    public class ToggleLikeByProductAndUserIdCommand : IRequest<bool>
    {
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
    }
}
