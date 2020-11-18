using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ravency.Core.Entities;
using Ravency.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ravency.Web.Areas.Catalog.ProductCategories
{
    public class Index
    {
        public class Query : IRequest<Result>
        {
            public Guid Id { get; set; }
        }

        public class Result
        {
            public Guid SelectedProductCategoryId { get; set; }
            public bool Delete { get; set; }
            public bool DeleteWithProducts { get; set; }
            public List<ProductCategory> ProductCategories { get; set; }

            public class ProductCategory
            {
                public Guid Id { get; set; }
                public string Name { get; set; }
                public bool MissData { get; set; }
                public bool AnyProductUsesThisCategory { get; set; }
            }
        }

        private class QueryHandler : IRequestHandler<Query, Result>
        {
            private readonly ApplicationDbContext _context;
            private readonly IConfigurationProvider _configuration;

            public QueryHandler(ApplicationDbContext context, IConfigurationProvider configuration)
            {
                _context = context;
                _configuration = configuration;
            }

            public async Task<Result> Handle(Query query, CancellationToken cancellationToken)
            {
                var categories = await _context.ProductCategories
                    .ProjectTo<Result.ProductCategory>(_configuration)
                    .ToListAsync();

                var languages = await _context.Languages
                    .Where(language => language.IsActive && language.IsDefault == false)
                    .ToListAsync();

                foreach (var category in categories)
                {
                    var categoryLocales = await _context.ProductCategoryLocales
                        .Where(categoryLocale => categoryLocale.CategoryId == category.Id)
                        .ToListAsync();
                    
                    if (categoryLocales.Count != languages.Count)
                    {
                        category.MissData = true;
                    }

                    var productsAny = await _context.Products
                        .Where(product => product.CategoryId == category.Id)
                        .AnyAsync();

                    if (productsAny)
                    {
                        category.AnyProductUsesThisCategory = true;
                    }
                }

                return new Result
                {
                    ProductCategories = categories
                };
            }
        }
    }
}
