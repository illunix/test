using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ravency.Application.ProductCategories.DTO;
using Ravency.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ravency.Application.ProductCategories.Queries.GetProductCategories
{
    public class GetProductCategoriesQueryHandler : IRequestHandler<GetProductCategoriesQuery, IEnumerable<ProductCategoryDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfigurationProvider _configuration;

        public GetProductCategoriesQueryHandler(ApplicationDbContext context, IConfigurationProvider configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IEnumerable<ProductCategoryDto>> Handle(GetProductCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _context.ProductCategories
                .ProjectTo<ProductCategoryDto>(_configuration)
                .ToListAsync();

            return categories;
        }
    }
}
