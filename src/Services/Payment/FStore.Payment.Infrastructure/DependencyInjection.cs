using FStore.Payment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace FStore.Payment.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddDbContext<PaymentDbContext>(options =>
      options.UseSqlServer(configuration.GetConnectionString("PaymentDb"))
    );
    return services;
  }
}