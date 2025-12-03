using MediatR;
using MeeshoWebClone.Models;

namespace MeeshoWebClone.Queries
{
    public class GetCategoriesListQuery : IRequest<List<ProductCategory>>
    {

    }
}
