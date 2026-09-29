using System.Text.Json;
using FStore.Basket.Domain.Entities;
using StackExchange.Redis;

namespace FStore.Basket.Infrastructure.Data;

public class RedisBasketRepository : IBasKetRepository
{
  private readonly IDatabase _database;

  public RedisBasketRepository(IConnectionMultiplexer redis)
  {
    _database = redis.GetDatabase();
  }
  public async Task<ShoppingCart?> GetBasketAsync(Guid userId)
  {
    var data = await _database.StringGetAsync(userId.ToString());
    return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<ShoppingCart>(data.ToString());
  }

  public async Task SaveBasketAsync(ShoppingCart cart)
  {
    var data = JsonSerializer.Serialize(cart);
    await _database.StringSetAsync(cart.UserId.ToString(), data, TimeSpan.FromDays(7));
  }
}