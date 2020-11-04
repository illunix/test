using MediatR;
using Ravency.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ravency.Application.ProductCategories.Commands.AddProductCategory
{
    public class DeleteProductCategoryCommandHandler : AsyncRequestHandler<DeleteProductCategoryCommand>
    {
        private readonly ApplicationDbContext _context;

        public DeleteProductCategoryCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        protected override async Task Handle(DeleteProductCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.ProductCategories
                .FindAsync(request.Id);

            _context.ProductCategories
                .Remove(category);

            await _context.SaveChangesAsync();
        }
    }
}
