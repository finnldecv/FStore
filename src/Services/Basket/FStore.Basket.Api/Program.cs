using FStore.Basket.Application;
using FStore.Basket.Application.Consumers;
using FStore.Basket.Infrastructure;
using FStore.Common.Middleware;
using FStore.EventBus;
using FStore.EventBus.Events;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IEventBus, MassTransitEventBus>();

builder.Services.AddMassTransit(x =>
{
  x.AddConsumer<ProductPriceChangedConsumer>();
  x.AddConsumer<ProductDeletedConsumer>();

  x.UsingRabbitMq((context, cfg) =>
  {
    cfg.Host(builder.Configuration.GetConnectionString("RabbitMq") ?? "localhost");
    cfg.ConfigureEndpoints(context);
  });
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