using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using FluentValidation.Resources;
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
    public class Edit 
    {
        public class Query : IRequest<Command>
        {
            public Guid Id { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(m => m.Id).NotNull();
            }
        }

        public class QueryHandler : IRequestHandler<Query, Command>
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;

            public QueryHandler(ApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Command> Handle(Query query, CancellationToken cancellationToken)
            {
                var categories = await _context.ProductCategories
                    .ToListAsync();

                var category = await _context.ProductCategories
                    .Where(category => category.Id == query.Id)
                    .SingleOrDefaultAsync();

                var categoryLocales = await _context.ProductCategoryLocales
                    .Where(categoryLocale => categoryLocale.CategoryId == query.Id)
                    .ToListAsync();

                var languages = await _context.Languages
                    .Where(language => language.IsActive)
                    .ProjectTo<Language<ProductCategory>>(_mapper.ConfigurationProvider)
                    .OrderByDescending(x => x.IsDefault)
                    .ThenBy(language => language.Name)
                    .ToListAsync();

                foreach (var language in languages)
                {
                    language.Data = new ProductCategory();

                    if (language.IsDefault)
                    {
                        language.Data.Name = category.Name;
                    }
                    else
                    {
                        foreach (var categoryLocale in categoryLocales)
                        {
                            if (language.Id == categoryLocale.LanguageId)
                            {
                                language.Data.Name = categoryLocale.Name;
                            }
                        }
                    }
                }

                return new Command
                {
                    Languages = languages,
                    ProductCategories = categories
                };
            }
        }

        public class Command : IRequest
        {
            public Guid Id { get; set; }
            public List<Language<ProductCategory>> Languages { get; set; }
            public List<ProductCategory> ProductCategories { get; set; }
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

            protected override async Task Handle(Command command, CancellationToken cancellationToken)
            {
                var category = await _context.ProductCategories
                    .FindAsync(command.Id);

                foreach (var language in command.Languages)
                {
                    if (language.IsDefault)
                    {
                        _mapper.Map(language, category);

                        _context.Update(category);
                    }
                    else
                    {
                        var categoryLocale = await _context.ProductCategoryLocales
                            .Where(x => x.CategoryId == category.Id && x.LanguageId == language.Id)
                            .SingleOrDefaultAsync();

                        if (categoryLocale != null)
                        {
                            _mapper.Map(language, categoryLocale);

                            _context.Update(categoryLocale);
                        }
                        else
                        {
                            var x = _mapper.Map<Language<ProductCategory>, ProductCategoryLocale>(language);

                            x.CategoryId = category.Id;

                            _context.Add(x);
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
