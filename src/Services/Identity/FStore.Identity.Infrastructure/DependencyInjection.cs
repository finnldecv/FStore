using FStore.Identity.Application.Services;
using FStore.Identity.Infrastructure.Data;
using FStore.Identity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FStore.Identity.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddDbContext<IdentityDbContext>(options =>
      options.UseSqlServer(configuration.GetConnectionString("IdentityDb"))
    );
    services.AddScoped<ITokenService, TokenService>();

    return services;
  }
}