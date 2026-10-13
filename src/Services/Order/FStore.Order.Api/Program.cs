using FStore.Common.Middleware;
using FStore.EventBus;
using FStore.Order.Application;
using FStore.Order.Application.Consumers;
using FStore.Order.Infrastructure;
using FStore.Order.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IEventBus, MassTransitEventBus>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BasketCheckoutConsumer>();
    x.AddConsumer<StockReservationFailedConsumer>();

    if (builder.Environment.IsEnvironment("Testing"))
    {
        x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();