using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ravency.Web.Areas.Configuration.Languages;

namespace Ravency.Web.Areas.Configuration.Languages
{
    [Area("Configuration")]
    [Route("[area]/[controller]/[action]")]
#if RELEASE
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "admin")]
#endif
    public class LanguagesController : Controller
    {
        private readonly IMediator _mediator;

        public LanguagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
            => View(await _mediator.Send(new Index.Query()));

        public async Task<IActionResult> Add()
            => View(await _mediator.Send(new Add.Query()));

        [HttpPost]
        public async Task<ActionResult> Add(Add.Command request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            await _mediator.Send(request);

            TempData["ToastrSuccess"] = "Successfully added new language.";

            return View(request);
        }

        public async Task<IActionResult> Edit(Edit.Query request)
        {
            var command = await _mediator.Send(request);

            if (!command.Languages.Any(language => language.Id == request.Id))
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

            TempData["ToastrSuccess"] = "Successfully updated product category.";

            return RedirectToAction(nameof(Edit), new { id = request.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Delete.Command request)
        {
            await _mediator.Send(request);

            TempData["ToastrSuccess"] = "Successfully deleted language.";

            return RedirectToAction(nameof(Index));
        }
    }
}
