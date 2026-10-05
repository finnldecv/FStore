using Docker.DotNet.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Testcontainers.Redis;

namespace FStore.Basket.IntegrationTests.Fixtures;

public class BasketApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
  private readonly RedisContainer _redis = new RedisBuilder("redis:7-alpine").Build();
  public async Task InitializeAsync()
  {
    await _redis.StartAsync();
  }

  async Task IAsyncLifetime.DisposeAsync()
  {
    await _redis.DisposeAsync();
    await base.DisposeAsync();
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.ConfigureServices(services =>
    {
      var descriptor = services.SingleOrDefault(
        d => d.ServiceType == typeof(IConnectionMultiplexer)
      );
      if (descriptor != null) services.Remove(descriptor);

      services.AddSingleton<IConnectionMultiplexer>(
        ConnectionMultiplexer.Connect(_redis.GetConnectionString())
      );
    });
  }
}