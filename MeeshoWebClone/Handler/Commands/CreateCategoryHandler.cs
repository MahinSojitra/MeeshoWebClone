using MediatR;
using MeeshoWebClone.Commands;
using MeeshoWebClone.Data;
using MeeshoWebClone.Models;
using Microsoft.EntityFrameworkCore;

namespace MeeshoWebClone.Handler.Commands
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, bool>
    {
        private readonly MeeshoAppDbContext _context;

        public CreateCategoryHandler(MeeshoAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var newCategory = new ProductCategory
            {
                Name = request.Name
            };

            await _context.ProductCategories.AddAsync(newCategory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
