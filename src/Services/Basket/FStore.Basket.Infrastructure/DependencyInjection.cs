using FStore.Basket.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FStore.Basket.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddSingleton<IConnectionMultiplexer>(sp =>
    {
      var redisConnection = configuration.GetConnectionString("BasketDb");
      return ConnectionMultiplexer.Connect(redisConnection!);
    });
    services.AddScoped<IBasKetRepository, RedisBasketRepository>();
    return services;
  }
}