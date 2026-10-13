using FStore.Order.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FStore.Order.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddDbContext<OrderDbContext>(options =>
      options.UseSqlServer(configuration.GetConnectionString("OrderDb"))
    );

    return services;
  }
}