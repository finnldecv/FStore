using FStore.EventBus.Events;
using FStore.Order.Application.Features.Orders.Commands.CancelOrder;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FStore.Order.Application.Consumers;

public class StockReservationFailedConsumer : IConsumer<StockReservationFailedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<StockReservationFailedConsumer> _logger;

    public StockReservationFailedConsumer(
        IMediator mediator,
        ILogger<StockReservationFailedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<StockReservationFailedEvent> context)
    {
        var evt = context.Message;
        _logger.LogWarning(
            "Stock reservation failed for order {OrderId}: {Reason}",
            evt.OrderId, evt.Reason);

        await _mediator.Send(
            new CancelOrderCommand(evt.OrderId, evt.Reason),
            context.CancellationToken);
    }
}