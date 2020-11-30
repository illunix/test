using AutoMapper;
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
        public record Query : IRequest<Model>
        {
        }

        public class QueryHandler : IRequestHandler<Query, Model>
        {
            private readonly ApplicationDbContext _context;
            private readonly IConfigurationProvider _configuration;

            public QueryHandler(ApplicationDbContext context, IConfigurationProvider configuration)
            {
                _context = context;
                _configuration = configuration;
            }

            public async Task<Model> Handle(Query query, CancellationToken cancellationToken)
            {
                var languages = await _context.Languages
                    .Where(language => language.IsActive)
                    .ProjectTo<Language<ProductCategory>>(_configuration)
                    .OrderByDescending(x => x.IsDefault)
                    .ThenBy(language => language.Name)
                    .ToListAsync();

                return new Model
                {
                    Languages = languages
                };
            }
        }

        public record Command : IRequest
        {
        }

        public record Model : Command
        {
            public List<Language<ProductCategory>> Languages;
        }

        public class CommandValidator : AbstractValidator<Model>
        {
            public CommandValidator(ApplicationDbContext context)
            {
                RuleFor(command => command.Languages)
                    .NotNull();

                RuleForEach(command => command.Languages)
                    .ChildRules(languages =>
                    {
                        languages.RuleFor(language => language.Data.Name)
                            .NotEmpty().WithMessage("Please enter category name.")
                            .MustAsync(async (language, name, cancellationToken) =>
                            {
                                var exist = false;

                                if (language.IsDefault)
                                {
                                    exist = await context.ProductCategories
                                        .AnyAsync(x => x.Name == name);
                                }
                                else
                                {
                                    exist = await context.ProductCategoryLocales
                                        .AnyAsync(x => x.Name == name);
                                }

                                return !exist;
                            }).WithMessage("Category with this name already exist");
                    });
            }
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

                var languages = await _context.Languages
                    .ProjectTo<Language<ProductCategory>>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                foreach (var language in languages)
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