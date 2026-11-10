using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

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
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.Run();