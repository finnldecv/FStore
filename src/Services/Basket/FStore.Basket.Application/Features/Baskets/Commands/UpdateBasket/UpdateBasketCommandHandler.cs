using FStore.Basket.Domain.Entities;
using FStore.Basket.Infrastructure.Data;
using MediatR;

namespace FStore.Basket.Application.Features.Baskets.Commands.UpdateBasket;

public class UpdateBasketCommandHandler : IRequestHandler<UpdateBasketCommand, bool>
{
  private readonly IBasketRepository _repository;

  public UpdateBasketCommandHandler(IBasketRepository repository)
  {
    _repository = repository;
  }
  public async Task<bool> Handle(UpdateBasketCommand request, CancellationToken ct)
  {
    var existing = await _repository.GetBasketAsync(request.UserId);
    if (existing is null) return false;

    existing.Items = request.Items.Select(i => new ShoppingCartItem
    {
      ProductId = i.ProductId,
      ProductName = i.ProductName,
      Price = i.Price,
      Quantity = i.Quantity
    }).ToList();

    await _repository.SaveBasketAsync(existing);
    return true;
  }
}