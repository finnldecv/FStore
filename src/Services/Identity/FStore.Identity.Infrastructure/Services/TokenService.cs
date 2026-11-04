using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FStore.Identity.Application.Services;
using FStore.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FStore.Identity.Infrastructure.Services;

public class TokenService : ITokenService
{
  private readonly IConfiguration _config;
  private readonly UserManager<ApplicationUser> _userManager;

  public TokenService(IConfiguration config, UserManager<ApplicationUser> userManager)
  {
    _config = config;
    _userManager = userManager;
  }
  public async Task<string> GenerateToken(ApplicationUser user)
  {
    var userRoles = await _userManager.GetRolesAsync(user);
    var claims = new List<Claim>
    {
      new Claim(JwtRegisteredClaimNames.Sub, user.Id),
      new Claim(JwtRegisteredClaimNames.Email, user.Email!),
      new Claim(ClaimTypes.Name, user.UserName!)
    };

    foreach (var role in userRoles)
    {
      claims.Add(new Claim(ClaimTypes.Role, role));
    }

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