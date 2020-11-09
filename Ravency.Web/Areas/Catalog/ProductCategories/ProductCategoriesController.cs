using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ravency.Web.Areas.Catalog.ProductCategories;
using Ravency.Core.Entities;
using Ravency.Infrastructure.Data;

namespace Ravency.Web.Areas.Catalog.Controllers
{
    [Area("Catalog")]
    [Route("[area]/product-categories/[action]")]
    public class ProductCategoriesController : Controller
    {
        private readonly IMediator _mediator;

        public ProductCategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<IActionResult> Add()
        {
            var command = await _mediator.Send(new Add.Query());

            return View(command);
        }

        [HttpPost]
        public async Task<ActionResult> Add(Add.Command command)
        {
            if (!ModelState.IsValid)
            {
                return View(command);
            }
            else
            {
                await _mediator.Send(command);

                TempData["ToastrSuccess"] = "Successfully added new product category.";
                return View(command);
            }
        }
    }
}
