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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Delete.Command request)
        {
            await _mediator.Send(request);

            TempData["ToastrSuccess"] = "Successfully deleted product category.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteWithProducts(DeleteWithProducts.Command request)
        {
            await _mediator.Send(request);

            TempData["ToastrSuccess"] = "Successfully deleted product category with products.";

            return RedirectToAction(nameof(Index));
        }
    }
}