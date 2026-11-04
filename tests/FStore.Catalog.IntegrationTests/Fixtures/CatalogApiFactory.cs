using FStore.Catalog.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.SqlClient;
using Respawn;
using Respawn.Graph;
using Testcontainers.MsSql;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authentication;
using FStore.Catalog.IntegrationTests.Auth;

namespace FStore.Catalog.IntegrationTests.Fixtures;

public class CatalogApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
  private readonly MsSqlContainer _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
    .WithPassword("FStore_Test_2026!")
    .Build();
  private Respawner? _respawner;
  public CatalogApiFactory()
  {
    Environment.SetEnvironmentVariable("USE_IN_MEMORY_MASSTRANSIT", "true");
  }
  public async Task InitializeAsync()
  {
    await _dbContainer.StartAsync();

    using var scope = Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await db.Database.MigrateAsync();
  }
  async Task IAsyncLifetime.DisposeAsync()
  {
    await _dbContainer.DisposeAsync();
    await base.DisposeAsync();
  }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Testing");

    builder.ConfigureAppConfiguration((context, config) =>
    {
      config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            {"JwtSettings:Key", "super_secret_key_that_is_at_least_32_characters_long!"},
            {"JwtSettings:Issuer", "FStore.Identity"},
            {"JwtSettings:Audience", "FStore.Client"}
        });
    });
    builder.ConfigureLogging(logging =>
    {
      logging.ClearProviders();
      logging.AddConsole();
      logging.SetMinimumLevel(LogLevel.Warning);
    });

    builder.ConfigureServices(services =>
    {
      services.RemoveAll<AuthenticationSchemeOptions>();
      services.RemoveAll<IAuthenticationHandlerProvider>();

      services.AddAuthentication(options =>
       {
         options.DefaultAuthenticateScheme = "Test";
         options.DefaultChallengeScheme = "Test";
       })
       .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

      var descriptor = services.SingleOrDefault(
        d => d.ServiceType == typeof(DbContextOptions<CatalogDbContext>)
      );
      if (descriptor != null) services.Remove(descriptor);

      services.AddDbContext<CatalogDbContext>(options =>
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
        TablesToIgnore = new Table[] { "_EFMigrationsHistory" }
      });
    }

    using var connection = new SqlConnection(_dbContainer.GetConnectionString());
    await connection.OpenAsync();
    await _respawner.ResetAsync(connection);
  }
}
