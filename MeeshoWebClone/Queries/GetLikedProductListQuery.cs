using MediatR;
using MeeshoWebClone.ViewModels;

namespace MeeshoWebClone.Queries
{
    public class GetLikedProductListQuery : IRequest<List<LikedProductViewModel>>
    {
        public Guid UserId { get; set; }
    }
}
