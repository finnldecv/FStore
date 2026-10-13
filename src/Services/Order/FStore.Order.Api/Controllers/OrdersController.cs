using FStore.Order.Application.Features.Orders.Commands.CancelOrder;
using FStore.Order.Application.Features.Orders.Commands.CreateOrder;
using FStore.Order.Application.Features.Orders.Queries.GetOrderById;
using FStore.Order.Application.Features.Orders.Queries.GetOrdersByUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FStore.Order.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
  private readonly IMediator _mediator;
  public OrdersController(IMediator mediator) => _mediator = mediator;

  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    var result = await _mediator.Send(new GetOrderByIdQuery(id));
    return result is null ? NotFound() : Ok(result);
  }

  [HttpGet("user/{userId:guid}")]
  public async Task<IActionResult> GetByUser(Guid userId)
      => Ok(await _mediator.Send(new GetOrdersByUserQuery(userId)));

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateOrderCommand command)
  {
    var order = await _mediator.Send(command);
    return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelRequest request)
  {
    var cancelled = await _mediator.Send(new CancelOrderCommand(id, request.Reason));
    return cancelled ? NoContent() : NotFound();
  }
}

public record CancelRequest(string Reason);