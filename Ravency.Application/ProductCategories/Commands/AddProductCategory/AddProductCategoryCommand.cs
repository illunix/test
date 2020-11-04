using MediatR;
using Ravency.Application.ProductCategories.DTO;
using Ravency.Application.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ravency.Application.ProductCategories.Commands.AddProductCategory
{
    public class AddProductCategoryCommand : IRequest
    {
        public int Gender { get; set; }
        public LanguageDto Language { get; set; }
        public IEnumerable<LanguageDto> Languages { get; set; }
    }
}
