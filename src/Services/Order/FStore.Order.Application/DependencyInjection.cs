using System.Reflection;
using FStore.Order.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FStore.Order.Application;

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