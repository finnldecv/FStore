using FStore.Basket.Domain.Entities;
using FStore.Basket.Infrastructure.Data;
using MediatR;

namespace FStore.Basket.Application.Features.Baskets.Queries.GetBasket;

public class GetBasketQueryHandler : IRequestHandler<GetBasketQuery, ShoppingCart?>
{
  private readonly IBasketRepository _repository;

  public GetBasketQueryHandler(IBasketRepository repository)
  {
    _repository = repository;
  }
  public Task<ShoppingCart?> Handle(GetBasketQuery request, CancellationToken ct)
  {
    return _repository.GetBasketAsync(request.UserId);
  }
}