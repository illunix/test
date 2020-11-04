using AutoMapper;
using MediatR;
using MediatR.Pipeline;
using Ravency.Application.ProductCategories.Commands.AddProductCategory;
using Ravency.Core.Entities;
using Ravency.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ravency.Application.ProductCategories.Processors
{
    public class AddProductCategoryCommandPostProcessor : IRequestPostProcessor<AddProductCategoryCommand, Unit>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AddProductCategoryCommandPostProcessor(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task Process(AddProductCategoryCommand request, Unit response, CancellationToken cancellationToken)
        {
            var categoryLocale = _mapper.Map<AddProductCategoryCommand, ProductCategoryLocale>(request);

            _context.ProductCategoryLocales
                .Add(categoryLocale);

            await _context.SaveChangesAsync();
        }
    }
}
