using FStore.Basket.Infrastructure.Data;
using FStore.EventBus;
using FStore.EventBus.Events;
using MassTransit;
using MediatR;

namespace FStore.Basket.Application.Features.Baskets.Commands.CheckoutBasket;

public class CheckoutBasketCommandHandler : IRequestHandler<CheckoutBasketCommand, bool>
{
  private readonly IBasketRepository _repository;
  private readonly IEventBus _eventBus;

  public CheckoutBasketCommandHandler(IBasketRepository repository, IEventBus eventBus)
  {
    _repository = repository;
    _eventBus = eventBus;
  }
  public async Task<bool> Handle(CheckoutBasketCommand request, CancellationToken ct)
  {
    var basket = await _repository.GetBasketAsync(request.UserId);

    if (basket is null || !basket.Items.Any())
      return false;

    var checkoutEvent = new BasketCheckoutEvent
    {
      UserId = request.UserId,
      ShippingAddress = request.ShippingAddress,
      Notes = request.Notes,
      Items = basket.Items.Select(i => new BasketCheckoutItem
      {
        ProductId = i.ProductId,
        ProductName = i.ProductName,
        UnitPrice = i.Price,
        Quantity = i.Quantity
      }).ToList()
    };

    await _eventBus.PublishAsync(checkoutEvent, ct);

    await _repository.DeleteBasketAsync(request.UserId);

    return true;
  }
}