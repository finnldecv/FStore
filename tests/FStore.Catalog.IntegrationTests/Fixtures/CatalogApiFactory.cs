using FStore.Catalog.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace FStore.Catalog.IntegrationTests.Fixtures;

public class CatalogApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
  private readonly MsSqlContainer _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
    .WithPassword("FStore_Test_2026!")
    .Build();
  public async Task InitializeAsync()
  {
    await _dbContainer.StartAsync();
  }

  async Task IAsyncLifetime.DisposeAsync()
  {
    await _dbContainer.DisposeAsync();
    await base.DisposeAsync();
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.ConfigureServices(services =>
    {
      var descriptor = services.SingleOrDefault(
        d => d.ServiceType == typeof(DbContextOptions<CatalogDbContext>)
      );
      if (descriptor != null) services.Remove(descriptor);

      services.AddDbContext<CatalogDbContext>(options =>
        options.UseSqlServer(_dbContainer.GetConnectionString())
      );
    });
  }
}
