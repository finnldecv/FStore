using FStore.EventBus;
using FStore.EventBus.Events;
using FStore.Order.Application.Features.Orders.Commands.CreateOrder;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FStore.Order.Application.Consumers;

public class BasketCheckoutConsumer : IConsumer<BasketCheckoutEvent>
{
  private readonly IMediator _mediator;
  private readonly ILogger<BasketCheckoutConsumer> _logger;

  public BasketCheckoutConsumer(IMediator mediator, ILogger<BasketCheckoutConsumer> logger)
  {
    _mediator = mediator;
    _logger = logger;
  }

  public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
  {
    var evt = context.Message;
    _logger.LogInformation(
        "Received checkout event for user {UserId} with {ItemCount} items",
        evt.UserId, evt.Items.Count);

    var command = new CreateOrderCommand(
        evt.UserId,
        evt.ShippingAddress,
        evt.Notes,
        evt.Items.Select(i => new CreateOrderItemDto(
            i.ProductId, i.ProductName, i.UnitPrice, i.Quantity)).ToList());

    var order = await _mediator.Send(command, context.CancellationToken);

    _logger.LogInformation("Created order {OrderId} from basket checkout", order.Id);
  }
}