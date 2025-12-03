using MediatR;
using MeeshoWebClone.Commands;
using MeeshoWebClone.Data;
using Microsoft.EntityFrameworkCore;

namespace MeeshoWebClone.Handler.Commands
{
    public class CheckCategoryExistHandler : IRequestHandler<CheckCategoryExistCommand, bool>
    {
        private readonly MeeshoAppDbContext _context;

        public CheckCategoryExistHandler(MeeshoAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(CheckCategoryExistCommand request, CancellationToken cancellationToken)
        {
            var existingCategory = await _context.ProductCategories
                .FirstOrDefaultAsync(c => c.Name == request.Name, cancellationToken);

            return existingCategory != null;
        }
    }
}
