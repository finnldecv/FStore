using FStore.Basket.Domain.Entities;

namespace FStore.Basket.Infrastructure.Data;

public interface IBasketRepository
{
  Task<ShoppingCart?> GetBasketAsync(Guid userId);
  Task SaveBasketAsync(ShoppingCart cart);
}