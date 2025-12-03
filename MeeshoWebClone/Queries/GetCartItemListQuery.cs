using MediatR;
using MeeshoWebClone.Models;
using MeeshoWebClone.ViewModels;

namespace MeeshoWebClone.Queries
{
    public class GetCartItemListQuery : IRequest<List<CartItemViewModel>>
    {
        public Guid UserId { get; set; }
    }
}
