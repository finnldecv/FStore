using System.Text.Json;
using FStore.Basket.Domain.Entities;
using StackExchange.Redis;

namespace FStore.Basket.Infrastructure.Data;

public class RedisBasketRepository : IBasketRepository
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


    foreach (var item in cart.Items)
    {
      await _database.SetAddAsync($"product:{item.ProductId}:users", cart.UserId.ToString());
    }
  }

  public async Task DeleteBasketAsync(Guid userId)
  {
    var existing = await GetBasketAsync(userId);

    if (existing is not null)
    {
      foreach (var item in existing.Items)
      {
        await _database.SetRemoveAsync($"product:{item.ProductId}:users", userId.ToString());
      }
    }

    await _database.KeyDeleteAsync(userId.ToString());
  }

  public async Task<IEnumerable<Guid>> GetUserIdsByProductAsync(Guid productId)
  {
    var members = await _database.SetMembersAsync($"product:{productId}:users");

    return members
        .Where(m => !m.IsNullOrEmpty)
        .Select(m => Guid.Parse(m.ToString()));
  }
}