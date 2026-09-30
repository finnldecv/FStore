using FStore.Basket.Infrastructure.Data;
using MediatR;

namespace FStore.Basket.Application.Features.Baskets.Commands.DeleteBasket;

public class DeleteBasketCommandHandler : IRequestHandler<DeleteBasketCommand, bool>
{
  private readonly IBasketRepository _repository;

  public DeleteBasketCommandHandler(IBasketRepository repository)
  {
    _repository = repository;
  }
  public async Task<bool> Handle(DeleteBasketCommand request, CancellationToken ct)
  {
    var existing = await _repository.GetBasketAsync(request.UserId);
    if (existing is null) return false;

    await _repository.DeleteBasketAsync(request.UserId);
    return true;
  }
}