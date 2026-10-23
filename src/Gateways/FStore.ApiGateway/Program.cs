using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!))
      };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
  options.AddPolicy("Frontend", policy =>
  {
    policy.WithOrigins(
              "http://localhost:5173",
              "http://localhost:80",
              "http://localhost",
              "https://fstore.com"
          )
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
  });
});

var app = builder.Build();

app.UseCors("Frontend");

app.MapReverseProxy();

app.Run();