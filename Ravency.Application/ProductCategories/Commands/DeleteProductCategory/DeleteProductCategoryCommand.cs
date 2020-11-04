using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ravency.Application.ProductCategories.Commands.AddProductCategory
{
    public class DeleteProductCategoryCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
