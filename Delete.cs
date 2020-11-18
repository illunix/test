using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ravency.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ravency.Web.Areas.Catalog.ProductCategories
{
    public class Delete
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
            public bool DeleteProductsOnly { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(command => command.Id)
                    .NotNull();
            }
        }

        public class CommandHandler : AsyncRequestHandler<Command>
        {
            private readonly ApplicationDbContext _context;

            public CommandHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            protected override async Task Handle(Command command, CancellationToken cancellationToken)
            {
                if (command.DeleteProductsOnly)
                {
                    var products = await _context.Products
                        .Where(product => product.CategoryId == command.Id)
                        .ToListAsync();

                    foreach (var product in products)
                    {
                        _context.Products
                            .Remove(product);
                    }
                }
                else
                {
                    var category = await _context.ProductCategories
                        .FindAsync(command.Id);

                    _context.ProductCategories
                        .Remove(category);

                    var categoryLocales = await _context.ProductCategoryLocales
                        .Where(x => x.CategoryId == command.Id)
                        .ToListAsync();

                    if (categoryLocales.Any())
                    {
                        foreach (var categoryLocale in categoryLocales)
                        {
                            _context.ProductCategoryLocales
                                .Remove(categoryLocale);
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
