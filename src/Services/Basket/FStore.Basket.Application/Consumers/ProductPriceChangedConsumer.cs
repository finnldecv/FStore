using FStore.Basket.Infrastructure.Data;
using FStore.EventBus.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FStore.Basket.Application.Consumers;

public class ProductPriceChangedConsumer : IConsumer<ProductPriceChangedEvent>
{
  private readonly IBasketRepository _repository;
  private readonly ILogger<ProductPriceChangedConsumer> _logger;

  public ProductPriceChangedConsumer(IBasketRepository repository, ILogger<ProductPriceChangedConsumer> logger)
  {
    _repository = repository;
    _logger = logger;
  }
  public async Task Consume(ConsumeContext<ProductPriceChangedEvent> context)
  {
    var evt = context.Message;
    _logger.LogInformation("Price changed for {ProductId}: {01d} -> {New}", evt.ProductId, evt.OldPrice, evt.NewPrice);
  }
}