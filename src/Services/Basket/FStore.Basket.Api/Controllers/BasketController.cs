using FStore.Basket.Application.Features.Baskets.Commands.CreateBasket;
using FStore.Basket.Application.Features.Baskets.Commands.DeleteBasket;
using FStore.Basket.Application.Features.Baskets.Commands.UpdateBasket;
using FStore.Basket.Application.Features.Baskets.Queries.GetBasket;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FStore.Basket.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketController : ControllerBase
{
  private readonly IMediator _mediator;

  public BasketController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpGet("{userId:guid}")]
  public async Task<IActionResult> Get(Guid userId)
  {
    var basket = await _mediator.Send(new GetBasketQuery(userId));
    return basket is null ? NotFound() : Ok(basket);
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateBasketCommand command)
  {
    var result = await _mediator.Send(command);
    return Ok(result);
  }

  [HttpPut("{userId:guid}")]
  public async Task<IActionResult> Update(Guid userId, [FromBody] UpdateBasketCommand command)
  {
    if (userId != command.UserId) return BadRequest("Route id and body id do not match");
    var updated = await _mediator.Send(command);
    return updated ? NoContent() : NotFound();
  }

  [HttpDelete("{userId:guid}")]
  public async Task<IActionResult> Delete(Guid userId)
  {
    var deleted = await _mediator.Send(new DeleteBasketCommand(userId));
    return deleted ? NoContent() : NotFound();
  }
}