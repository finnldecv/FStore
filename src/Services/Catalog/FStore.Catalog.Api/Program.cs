using System.Text;
using FStore.Catalog.Application;
using FStore.Catalog.Infrastructure;
using FStore.Catalog.Infrastructure.Data;
using FStore.Common.Middleware;
using FStore.EventBus;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

var jwtKey = builder.Configuration["JwtSettings:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
  throw new InvalidOperationException(
      "JWT configuration missing: JwtSettings:Key"
  );
}

byte[] keyBytes;

try
{
  keyBytes = Convert.FromBase64String(jwtKey);
}
catch (FormatException)
{
  throw new InvalidOperationException(
      "JwtSettings:Key must be a valid Base64 string."
  );
}

if (keyBytes.Length < 32)
{
  throw new InvalidOperationException(
      "JWT signing key must contain at least 32 bytes."
  );
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      options.MapInboundClaims = false;

      options.TokenValidationParameters =
          new TokenValidationParameters
          {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer =
                  builder.Configuration["JwtSettings:Issuer"],

            ValidAudience =
                  builder.Configuration["JwtSettings:Audience"],

            IssuerSigningKey =
                  new SymmetricSecurityKey(keyBytes),

            NameClaimType = "name",
            RoleClaimType = "role",

            ClockSkew = TimeSpan.FromSeconds(30)
          };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

//app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();