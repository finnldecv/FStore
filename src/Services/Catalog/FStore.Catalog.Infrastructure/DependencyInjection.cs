using FStore.Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FStore.Catalog.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(
    this IServiceCollection services, IConfiguration configuration)
  {
    services.AddDbContext<CatalogDbContext>(options =>
      options.UseSqlServer(configuration.GetConnectionString("CatalogDb")));
    return services;
  }
}