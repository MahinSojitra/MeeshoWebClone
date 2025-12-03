using MediatR;
using MeeshoWebClone.Areas.Admin.ViewModels;
using MeeshoWebClone.Commands;
using MeeshoWebClone.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeeshoWebClone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IActionResult Index()
        {
            return View(_mediator.Send(new GetCategoriesListQuery()).Result);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCategoryCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingCategory = await _mediator.Send(new CheckCategoryExistCommand { Name = model.Name });
            if (existingCategory)
            {
                ModelState.AddModelError("Name", "Category already exists.");
                return View(model);
            }

            var result = await _mediator.Send(new CreateCategoryCommand { Name = model.Name });

            if (!result)
            {
                ModelState.AddModelError("Name", "Failed to create category.");
                return View(model);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteCategoryByIdCommand { Id = id });

            return RedirectToAction("Index");
        }
    }
}
