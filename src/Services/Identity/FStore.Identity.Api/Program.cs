using FStore.Identity.Domain.Entities;
using FStore.Identity.Infrastructure;
using FStore.Identity.Infrastructure.Data;
using FStore.Identity.Application;
using Microsoft.AspNetCore.Identity;
using FStore.Common.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddDataProtection();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddIdentityCore<ApplicationUser>()
  .AddRoles<IdentityRole>()
  .AddEntityFrameworkStores<IdentityDbContext>()
  .AddDefaultTokenProviders()
  .AddSignInManager();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
  await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

// app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();