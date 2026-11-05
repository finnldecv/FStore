using FStore.Common.Middleware;
using FStore.EventBus;
using FStore.Payment.Application.Consumers;
using FStore.Payment.Infrastructure;
using FStore.Payment.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
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
        var connectionString = builder.Configuration.GetConnectionString("RabbitMq");

        if (connectionString != null && connectionString.StartsWith("rabbitmq://"))
        {
          var uri = new Uri(connectionString);
          cfg.Host(uri);
        }
        else
        {
          cfg.Host(connectionString ?? "amqp://guest:guest@localhost:5672");
        }

        cfg.ConfigureEndpoints(context);
      });
  }
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
  await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
