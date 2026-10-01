using FStore.Basket.Infrastructure.Data;
using FStore.EventBus.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FStore.Basket.Application.Consumers;

public class ProductDeletedConsumer : IConsumer<ProductDeletedEvent>
{
  private readonly ILogger<ProductDeletedEvent> _logger;

  public ProductDeletedConsumer(ILogger<ProductDeletedEvent> logger)
  {
    _logger = logger;
  }
  public Task Consume(ConsumeContext<ProductDeletedEvent> context)
  {
    _logger.LogWarning("Product {ProductId} was deleted from catalog", context.Message.ProductId);
    return Task.CompletedTask;
  }
}