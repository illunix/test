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

        public async Task<IActionResult> Add(Add.Query request)
            => View(await _mediator.Send(request));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Add.Command request)
        {
            if (!ModelState.IsValid)
            {
                return View(await _mediator.Send(new Add.Query()));
            }

            await _mediator.Send(request);

            TempData["ToastrSuccess"] = "Successfully added new product category.";

            return View(await _mediator.Send(new Add.Query()));
        }

        public async Task<IActionResult> Edit(Edit.Query request)
        {
            var command = await _mediator.Send(request);

            if (!command.ProductCategories.Any(productCategory => productCategory.Id == request.Id))
            {
                Response.StatusCode = 404;
                return View("AdminLTE/_PageNotFound");
            }

            return View(command);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Edit.Command request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            await _mediator.Send(request);

            TempData["ToastrSuccess"] = "Successfully updated language.";

            return RedirectToAction(nameof(Edit), new { id = request.Id });
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