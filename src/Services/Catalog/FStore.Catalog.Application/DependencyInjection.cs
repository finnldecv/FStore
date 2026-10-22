using System.Reflection;
using FluentValidation;
using FStore.Catalog.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FStore.Catalog.Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    var assembly = Assembly.GetExecutingAssembly();

    services.AddMediatR(assembly);

    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

    services.AddValidatorsFromAssembly(assembly);

    return services;
  }
}