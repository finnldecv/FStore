using FStore.Basket.Domain.Entities;

namespace FStore.Basket.Infrastructure.Data;

public interface IBasKetRepository
{
  Task<ShoppingCart?> GetBasketAsync(Guid userId);
  Task SaveBasketAsync(ShoppingCart cart);
}