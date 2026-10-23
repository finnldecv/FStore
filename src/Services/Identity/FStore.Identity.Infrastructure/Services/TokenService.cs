using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FStore.Identity.Application.Services;
using FStore.Identity.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FStore.Identity.Infrastructure.Services;

public class TokenService : ITokenService
{
  private readonly IConfiguration _config;

  public TokenService(IConfiguration config)
  {
    _config = config;
  }
  public string GenerateToken(ApplicationUser user)
  {
    var claims = new[]
    {
      new Claim(JwtRegisteredClaimNames.Sub, user.Id),
      new Claim(JwtRegisteredClaimNames.Email, user.Email!),
      new Claim(ClaimTypes.Name, user.UserName!)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      issuer: _config["JwtSettings:Issuer"],
      audience: _config["JwtSettings:Audience"],
      claims: claims,
      expires: DateTime.UtcNow.AddHours(1),
      signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}