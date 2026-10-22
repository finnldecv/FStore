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
      var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

      var connectionString = isDocker
          ? "basketdb:6379"
          : configuration.GetConnectionString("BasketDb") ?? "localhost:6379";
      var options = ConfigurationOptions.Parse(connectionString);
      options.AbortOnConnectFail = false;

      return ConnectionMultiplexer.Connect(options);
    });

    services.AddScoped<IBasketRepository, RedisBasketRepository>();

    return services;
  }
}