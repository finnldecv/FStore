var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseCors("Frontend");

app.MapReverseProxy();

app.Run();