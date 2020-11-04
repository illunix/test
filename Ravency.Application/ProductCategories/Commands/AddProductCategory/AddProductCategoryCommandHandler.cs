using AutoMapper;
using MediatR;
using Ravency.Application.ProductCategories.DTO;
using Ravency.Application.Shared.DTO;
using Ravency.Core.Entities;
using Ravency.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ravency.Application.ProductCategories.Commands.AddProductCategory
{
    public class AddProductCategoryCommandHandler : AsyncRequestHandler<AddProductCategoryCommand>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AddProductCategoryCommandHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        protected override async Task Handle(AddProductCategoryCommand request, CancellationToken cancellationToken)
        {
            var dto = _mapper.Map<LanguageDto, ProductCategoryDto>(request.Language);

            var category = _mapper.Map<ProductCategoryDto, ProductCategory>(dto);

            _context.ProductCategories
                .Add(category);

            await _context.SaveChangesAsync();
        }
    }
}
