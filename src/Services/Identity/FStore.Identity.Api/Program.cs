using FStore.Identity.Domain.Entities;
using FStore.Identity.Infrastructure;
using FStore.Identity.Infrastructure.Data;
using FStore.Identity.Application;
using Microsoft.AspNetCore.Identity;
using FStore.Common.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddDataProtection()
  .PersistKeysToDbContext<IdentityDbContext>();

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

  var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
  var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

  if (!await roleManager.RoleExistsAsync("Admin"))
    await roleManager.CreateAsync(new IdentityRole("Admin"));
  if (!await roleManager.RoleExistsAsync("Customer"))
    await roleManager.CreateAsync(new IdentityRole("Customer"));

  var adminEmail = builder.Configuration["AdminSetting:Password"];
  var adminPassword = builder.Configuration["AdminSetting:Password"];
  var adminUser = await userManager.FindByEmailAsync(adminEmail!);
  if (adminUser == null)
  {
    var newAdmin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, FirstName = "System", LastName = "Admin" };
    await userManager.CreateAsync(newAdmin, adminPassword!);
    await userManager.AddToRoleAsync(newAdmin, "Admin");
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
}