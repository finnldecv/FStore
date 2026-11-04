using FStore.Identity.Domain.Entities;

namespace FStore.Identity.Application.Services;

public interface ITokenService
{
  Task<string> GenerateToken(ApplicationUser user);
}