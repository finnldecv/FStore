using FStore.Basket.Infrastructure.Data;
using FStore.EventBus.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FStore.Basket.Application.Consumers;

public class ProductDeletedConsumer : IConsumer<ProductDeletedEvent>
{
  private readonly IBasketRepository _repository;
  private readonly ILogger<ProductDeletedConsumer> _logger;

  public ProductDeletedConsumer(
      IBasketRepository repository,
      ILogger<ProductDeletedConsumer> logger)
  {
    _repository = repository;
    _logger = logger;
  }

  public async Task Consume(ConsumeContext<ProductDeletedEvent> context)
  {
    var productId = context.Message.ProductId;

    _logger.LogWarning("Product {ProductId} was deleted. Cleaning up carts.", productId);

    var affectedUserIds = await _repository.GetUserIdsByProductAsync(productId);

    foreach (var userId in affectedUserIds)
    {
      var cart = await _repository.GetBasketAsync(userId);
      if (cart is null) continue;

      var itemsBefore = cart.Items.Count;
      cart.Items = cart.Items
          .Where(i => i.ProductId != productId)
          .ToList();

      if (cart.Items.Count < itemsBefore)
      {
        await _repository.SaveBasketAsync(cart);
        _logger.LogInformation("Removed deleted product from user {UserId}'s cart.", userId);
      }
    }
  }
}