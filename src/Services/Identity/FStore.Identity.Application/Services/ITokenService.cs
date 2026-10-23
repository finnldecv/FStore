using FStore.Identity.Domain.Entities;

namespace FStore.Identity.Application.Services;

public interface ITokenService
{
  string GenerateToken(ApplicationUser user);
}