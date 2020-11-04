using MediatR;
using Ravency.Application.ProductCategories.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ravency.Application.ProductCategories.Queries.GetProductCategories
{
    public class GetProductCategoriesQuery : IRequest<IEnumerable<ProductCategoryDto>>
    {
        public Guid Id { get; set; }
    }
}
