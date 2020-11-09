using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using FluentValidation.Resources;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ravency.Core.Entities;
using Ravency.Application.Shared.DTO;
using Ravency.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Dynamic;

namespace Ravency.Application.ProductCategories.Commands
{
    public class AddProductCategory
    {
        public class Query : IRequest<Command>
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
                    .ProjectTo<LanguageDto<ProductCategory>>(_configuration)
                    .OrderByDescending(x => x.IsDefault)
                    .ThenBy(language => language.Name)
                    .ToListAsync();

                return new Command
                {
                    Languages = languages
                };
            }
        }

        public class Command : IRequest
        {
            public int Gender { get; set; }
            public List<LanguageDto<ProductCategory>> Languages { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
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

            protected override async Task Handle(Command command, CancellationToken cancellationToken)
            {
                foreach (var language in command.Languages)
                {
                    var category = new ProductCategory();

                    category.Id = Guid.NewGuid();

                    if (language.IsDefault)
                    {
                        category = _mapper.Map<LanguageDto<ProductCategory>, ProductCategory>(language);

                        _mapper.Map(command, category);

                        _context.ProductCategories
                            .Add(category);
                    }
                    else
                    {
                        var categoryLocale = _mapper.Map<LanguageDto<ProductCategory>, ProductCategoryLocale>(language);

                        categoryLocale.Id = Guid.NewGuid();

                        _mapper.Map(category, categoryLocale);

                        _context.ProductCategoryLocales
                            .Add(categoryLocale);
                    }

                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
