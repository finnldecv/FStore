using FStore.Identity.Domain.Entities;
using FStore.Identity.Application.Services;
using Microsoft.AspNetCore.Identity;
using MediatR;

namespace FStore.Identity.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly ITokenService _tokenService;

  public LoginCommandHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService)
  {
    _userManager = userManager;
    _tokenService = tokenService;
  }
  public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
  {
    var user = await _userManager.FindByEmailAsync(request.Email);
    if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
    {
      throw new Exception("Invalid email or password");
    }

    return _tokenService.GenerateToken(user);
  }
}