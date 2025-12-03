using MediatR;
using MeeshoWebClone.Commands;
using MeeshoWebClone.Data;

namespace MeeshoWebClone.Handler.Commands
{
    public class DeleteCategoryByIdHandler : IRequestHandler<DeleteCategoryByIdCommand, bool>
    {
        private readonly MeeshoAppDbContext _context;

        public DeleteCategoryByIdHandler(MeeshoAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteCategoryByIdCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.ProductCategories.FindAsync(request.Id);
            
            if (category == null)
            {
                return false;
            }

            _context.ProductCategories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
