using MediatR;

namespace MeeshoWebClone.Commands
{
    public class DeleteCategoryByIdCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
