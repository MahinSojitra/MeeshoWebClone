using MediatR;
using MeeshoWebClone.Data;
using MeeshoWebClone.Models;
using MeeshoWebClone.Queries;
using Microsoft.EntityFrameworkCore;

namespace MeeshoWebClone.Handler.Queries
{
    public class GetCategoriesListHandler : IRequestHandler<GetCategoriesListQuery, List<ProductCategory>>
    {
        private readonly MeeshoAppDbContext _context;
        
        public GetCategoriesListHandler(MeeshoAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductCategory>> Handle(GetCategoriesListQuery request, CancellationToken cancellationToken)
        {
            var categories = await _context.ProductCategories.ToListAsync(cancellationToken);
            return categories;
        }
    }
}
