using FStore.Catalog.Application;
using FStore.Catalog.Infrastructure;
using FStore.Catalog.Infrastructure.Data;
using FStore.Common.Middleware;
using FStore.EventBus;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IEventBus, MassTransitEventBus>();

builder.Services.AddMassTransit(x =>
{
  x.AddConsumers(typeof(Program).Assembly);

  if (builder.Environment.IsEnvironment("Testing") ||
  Environment.GetEnvironmentVariable("USE_IN_MEMORY_MASSTRANSIT") == "true")
  {
    x.UsingInMemory((context, cfg) =>
    {
      cfg.ConfigureEndpoints(context);
    });
  }
  else
  {
    x.UsingRabbitMq((context, cfg) =>
      {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq") ?? "localhost");
        cfg.ConfigureEndpoints(context);
      });
  }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

//app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthorization();
app.MapControllers();


app.Run();