using MediatR;

namespace MeeshoWebClone.Commands
{
    public class CreateCategoryCommand : IRequest<bool>
    {
        public string Name { get; set; }
    }
}
