using FStore.Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Respawn;
using Respawn.Graph;
using Testcontainers.MsSql;

namespace FStore.Identity.IntegrationTests.Fixtures;

public class IdentityApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
  private readonly MsSqlContainer _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
    .WithPassword("FStore_Test_2026!")
    .Build();

  private Respawner? _respawner;
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
    builder.UseEnvironment("Testing");
    builder.ConfigureLogging(l => l.ClearProviders());

    builder.ConfigureServices(services =>
    {
      var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<IdentityDbContext>));
      if (descriptor != null) services.Remove(descriptor);

      services.AddDbContext<IdentityDbContext>(options =>
        options.UseSqlServer(_dbContainer.GetConnectionString())
      );
    });
  }

  public async Task ResetDatabaseAsync()
  {
    if (_respawner is null)
    {
      using var conn = new SqlConnection(_dbContainer.GetConnectionString());
      await conn.OpenAsync();
      _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
      {
        DbAdapter = DbAdapter.SqlServer,
        TablesToIgnore = new Table[] { "__EFMigrationsHistory" }
      });
    }

    using var connection = new SqlConnection(_dbContainer.GetConnectionString());
    await connection.OpenAsync();
    await _respawner.ResetAsync(connection);
  }
}