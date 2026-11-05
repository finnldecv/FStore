using FStore.EventBus;
using FStore.EventBus.Events;
using FStore.Payment.Domain.Entities;
using FStore.Payment.Infrastructure.Data;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FStore.Payment.Application.Consumers;

public class OrderConfirmedConsumer : IConsumer<OrderConfirmedEvent>
{
  private readonly PaymentDbContext _db;
  private readonly IEventBus _eventBus;
  private readonly ILogger<OrderConfirmedConsumer> _logger;

  public OrderConfirmedConsumer(PaymentDbContext db, IEventBus eventBus, ILogger<OrderConfirmedConsumer> logger)
  {
    _db = db;
    _eventBus = eventBus;
    _logger = logger;
  }

  public async Task Consume(ConsumeContext<OrderConfirmedEvent> context)
  {
    var evt = context.Message;
    _logger.LogInformation("Processing payment for Order {OrderId} Amount: {Amount}", evt.OrderId, evt.TotalAmount);

    // MOCK PAYMENT LOGIC: Decline if over $10,000
    bool isSuccess = evt.TotalAmount <= 10000m;
    string? failureReason = isSuccess ? null : "Credit card declined (Amount exceeds limit).";

    var payment = new PaymentRecord
    {
      OrderId = evt.OrderId,
      UserId = evt.UserId,
      Amount = evt.TotalAmount,
      Status = isSuccess ? "Success" : "Failed",
      FailureReason = failureReason
    };

    _db.PaymentRecords.Add(payment);
    await _db.SaveChangesAsync(context.CancellationToken);

    // Publish the result
    await _eventBus.PublishAsync(new PaymentProcessedEvent
    {
      OrderId = evt.OrderId,
      IsSuccessful = isSuccess,
      FailureReason = failureReason
    }, context.CancellationToken);
  }
}