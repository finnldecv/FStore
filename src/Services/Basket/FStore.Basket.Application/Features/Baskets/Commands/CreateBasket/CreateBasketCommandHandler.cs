using FStore.Basket.Domain.Entities;
using FStore.Basket.Infrastructure.Data;
using MediatR;

namespace FStore.Basket.Application.Features.Baskets.Commands.CreateBasket;

public class CreateBasketCommandHandler : IRequestHandler<CreateBasketCommand, bool>
{
  private readonly IBasketRepository _repository;

  public CreateBasketCommandHandler(IBasketRepository repository)
  {
    _repository = repository;
  }
  public async Task<bool> Handle(CreateBasketCommand request, CancellationToken cancellationToken)
  {
    var cart = new ShoppingCart
    {
      UserId = request.UserId,
      Items = request.Items.Select(i => new ShoppingCartItem
      {
        ProductId = i.ProductId,
        ProductName = i.ProductName,
        Price = i.Price,
        Quantity = i.Quantity
      }).ToList()
    };

    await _repository.SaveBasketAsync(cart);
    return true;
  }
}