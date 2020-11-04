using Ravency.Application.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ravency.Application.ProductCategories.DTO
{
    public class LanguageDto : BaseLanguageDto
    {
        public ProductCategoryDto ProductCategory { get; set; }
    }
}
