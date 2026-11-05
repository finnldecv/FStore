using FStore.EventBus.Events;
using FStore.Order.Application.Features.Orders.Commands.CancelOrder;
using FStore.Order.Infrastructure.Data;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FStore.Order.Application.Consumers;

public class PaymentProcessedConsumer : IConsumer<PaymentProcessedEvent>
{
  private readonly OrderDbContext _db;
  private readonly IMediator _mediator;

  public PaymentProcessedConsumer(OrderDbContext db, IMediator mediator)
  {
    _db = db;
    _mediator = mediator;
  }

  public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
  {
    var evt = context.Message;
    var order = await _db.Orders.FindAsync(new object[] { evt.OrderId }, context.CancellationToken);

    if (order is null) return;

    if (evt.IsSuccessful)
    {
      // Mark order as Confirmed (or Paid)
      order.MarkAsConfirmed();
      await _db.SaveChangesAsync(context.CancellationToken);
    }
    else
    {
      // Payment failed -> Cancel the order!
      await _mediator.Send(new CancelOrderCommand(order.Id, evt.FailureReason ?? "Payment failed"), context.CancellationToken);
    }
  }
}