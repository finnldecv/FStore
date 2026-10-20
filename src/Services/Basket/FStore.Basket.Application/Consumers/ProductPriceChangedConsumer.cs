using FStore.Basket.Infrastructure.Data;
using FStore.EventBus.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FStore.Basket.Application.Consumers;

public class ProductPriceChangedConsumer : IConsumer<ProductPriceChangedEvent>
{
  private readonly IBasketRepository _repository;
  private readonly ILogger<ProductPriceChangedConsumer> _logger;

  public ProductPriceChangedConsumer(
      IBasketRepository repository,
      ILogger<ProductPriceChangedConsumer> logger)
  {
    _repository = repository;
    _logger = logger;
  }

  public async Task Consume(ConsumeContext<ProductPriceChangedEvent> context)
  {
    var evt = context.Message;
    _logger.LogInformation("Price changed for {ProductId}: {Old} → {New}",
        evt.ProductId, evt.OldPrice, evt.NewPrice);

    var affectedUserIds = await _repository.GetUserIdsByProductAsync(evt.ProductId);

    foreach (var userId in affectedUserIds)
    {
      var cart = await _repository.GetBasketAsync(userId);
      if (cart is null) continue;

      var item = cart.Items.FirstOrDefault(i => i.ProductId == evt.ProductId);
      if (item is null) continue;

      item.Price = evt.NewPrice;
      await _repository.SaveBasketAsync(cart);

      _logger.LogInformation("Updated price in cart for user {UserId}", userId);
    }
  }
}