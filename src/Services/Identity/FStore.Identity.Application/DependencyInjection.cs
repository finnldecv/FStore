using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FStore.Identity.Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    var assembly = Assembly.GetExecutingAssembly();

    services.AddMediatR(assembly);
    return services;
  }
}