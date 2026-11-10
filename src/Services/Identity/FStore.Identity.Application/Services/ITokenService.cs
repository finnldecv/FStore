using FStore.Identity.Domain.Entities;

namespace FStore.Identity.Application.Services;

public interface ITokenService
{
  Task<string> GenerateToken(string userId, string email, string name, IList<string> roles);
}