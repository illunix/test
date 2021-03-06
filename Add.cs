﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ravency.Core.Entities;
using Ravency.Infrastructure.Data;
using Ravency.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ravency.Web.Areas.Catalog.ProductCategories
{
    public class Add
    {
        public record Query : IRequest<Command>
        {
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly ApplicationDbContext _context;
            private readonly IConfigurationProvider _configuration;

            public QueryHandler(ApplicationDbContext context, IConfigurationProvider configuration)
            {
                _context = context;
                _configuration = configuration;
            }

            public async Task<Command> Handle(Query query, CancellationToken cancellationToken)
            {
                var languages = await _context.Languages
                    .Where(language => language.IsActive)
                    .ProjectTo<Language<ProductCategory>>(_configuration)
                    .OrderByDescending(x => x.IsDefault)
                    .ThenBy(language => language.Name)
                    .ToListAsync();

                return new Command
                {
                    Languages = languages
                };
            }
        }

        public record Command : IRequest
        {
            public List<Language<ProductCategory>> Languages;
        }

        public class CommandHandler : AsyncRequestHandler<Command>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;

            public CommandHandler(ApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            protected override async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var categoryId = new Guid();

                foreach (var language in request.Languages)
                {
                    if (language.IsDefault)
                    {
                        var category = _mapper.Map<Language<ProductCategory>, ProductCategory>(language);

                        _mapper.Map(request, category);

                        _context.ProductCategories
                            .Add(category);

                        categoryId = category.Id;
                    }
                    else
                    {
                        var categoryLocale = _mapper.Map<Language<ProductCategory>, ProductCategoryLocale>(language);

                        categoryLocale.CategoryId = categoryId;

                        _context.ProductCategoryLocales
                            .Add(categoryLocale);
                    }
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}