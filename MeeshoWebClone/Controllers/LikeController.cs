using MediatR;
using MeeshoWebClone.Commands;
using MeeshoWebClone.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MeeshoWebClone.Controllers
{
    [Authorize]
    [Route("Like")]
    public class LikeController : Controller
    {
        private readonly IMediator _mediator;

        public LikeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IActionResult Index()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cartItems = _mediator.Send(new GetLikedProductListQuery { UserId = userId } ).Result;

            return View(cartItems);
        }

        [HttpGet("Toggle/{id:guid}")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _mediator.Send(new ToggleLikeByProductAndUserIdCommand
            {
                UserId = userId,
                ProductId = id
            });

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
