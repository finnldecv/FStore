using FStore.Basket.Application.Features.Baskets.Commands.CreateBasket;
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

  [HttpPost]
  public async Task<IActionResult> CreateBasket([FromBody] CreateBasketCommand command)
  {
    var result = await _mediator.Send(command);
    return Ok(result);
  }
}