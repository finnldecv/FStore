using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FStore.Identity.Application.Services;
using FStore.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FStore.Identity.Infrastructure.Services;

public class TokenService : ITokenService
{
  private readonly JwtOptions _options;

  public TokenService(IOptions<JwtOptions> options)
  {
    _options = options.Value;
  }
  public async Task<string> GenerateToken(string userId, string email, string name, IList<string> roles)
  {
    var now = DateTime.UtcNow;
    var claims = new List<Claim>
    {
      new Claim(JwtRegisteredClaimNames.Sub, userId),
      new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
      new("name", name ?? email ?? userId),
      new(
        JwtRegisteredClaimNames.Iat,
        EpochTime.GetIntDate(DateTime.UtcNow).ToString(),
        ClaimValueTypes.Integer64
      ),
    };

    if (!string.IsNullOrWhiteSpace(email))
    {
      claims.Add(new Claim(JwtRegisteredClaimNames.Email, email!));

    }

    var roleClaims = roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(role => new Claim("role", role));

    claims.AddRange(roleClaims);

    var keyBytes = Convert.FromBase64String(_options.Key);

    if (keyBytes.Length < 32)
    {
      throw new InvalidOperationException("JWT signing key must contain at least 32 bytes.");
    }

    var key = new SymmetricSecurityKey(keyBytes);
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      issuer: _options.Issuer,
      audience: _options.Audience,
      claims: claims,
      notBefore: now,
      expires: now.AddMinutes(_options.AccessTokenMinutes),
      signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}