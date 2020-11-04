using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Ravency.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ravency.Application.ProductCategories.Commands.AddProductCategory
{
    public class AddProductCategoryCommandValidator : AbstractValidator<AddProductCategoryCommand>
    {
        public AddProductCategoryCommandValidator(ApplicationDbContext context)
        {
            RuleForEach(language => language.Languages)
                .ChildRules(languages =>
                {
                    languages.RuleFor(language => language.ProductCategory.Name)
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
}
