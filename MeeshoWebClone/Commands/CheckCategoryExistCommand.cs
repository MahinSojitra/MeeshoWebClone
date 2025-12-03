using MediatR;

namespace MeeshoWebClone.Commands
{
    public class CheckCategoryExistCommand : IRequest<bool>
    {
        public string Name { get; set; }
    }
}
