using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Ravency.Web.Areas.Catalog.ProductCategories
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

        public async Task<IActionResult> Index()
            => View(await _mediator.Send(new Index.Query()));

        public async Task<IActionResult> Add()
            => View(await _mediator.Send(new Add.Query()));

        [HttpPost]
        public async Task<ActionResult> Add(Add.Command command)
        {
            if (!ModelState.IsValid)
            {
                return View(command);
            }

            await _mediator.Send(command);

            TempData["ToastrSuccess"] = "Successfully added new product category.";
            return View(command);
        }

        public async Task<IActionResult> Edit(Edit.Query query)
        {
            var command = await _mediator.Send(query);

            if (!command.ProductCategories.Any(productCategory => productCategory.Id == query.Id))
            {
                Response.StatusCode = 404;
                return View("AdminLTE/_PageNotFound");
            }

            return View(command);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Edit.Command command)
        {
            if (!ModelState.IsValid)
            {
                return View(command);
            }

            await _mediator.Send(command);

            TempData["ToastrSuccess"] = "Successfully updated product category.";
            return View(command);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(Index.Result model)
        {
            var productCategories = (await _mediator.Send(new Index.Query())).ProductCategories;

            var productCategory = productCategories
                .Where(category => category.Id == model.SelectedProductCategoryId)
                .SingleOrDefault();

            if (productCategory is null)
            {
                Response.StatusCode = 500;
                return View("AdminLTE/_SomethingWentWrong");
            }

            if (model.Delete)
            {
                await _mediator.Send(new Index.Command { Id = model.SelectedProductCategoryId, Delete = true });

            }
            else if (model.DeleteWithProducts)
            {
                await _mediator.Send(new Index.Command { Id = model.SelectedProductCategoryId, DeleteWithProducts = true });
            }

            productCategories = (await _mediator.Send(new Index.Query())).ProductCategories;

            model.ProductCategories = productCategories;

            TempData["ToastrSuccess"] = "Successfully deleted product category.";

            return View("Index", model);
        }
    }
}