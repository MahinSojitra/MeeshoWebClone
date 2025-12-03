using MediatR;
using MeeshoWebClone.Commands;
using MeeshoWebClone.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MeeshoWebClone.Controllers
{
    [Authorize]
    [Route("Cart")]
    public class CartController : Controller
    {
        private readonly IMediator _mediator;

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IActionResult Index()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cartItems = _mediator.Send(new GetCartItemListQuery { UserId = userId }).Result;

            return View(cartItems);
        }

        [HttpGet("Update/{id:guid}/{change:int}")]
        public async Task<IActionResult> Update(Guid id, int change)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _mediator.Send(new UpdateCartByProductIdAndChangeCommand
            {
                ProductId = id,
                Change = change,
                UserId = userId
            });
            
            if (result == 0)
            {
                return NotFound();
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
