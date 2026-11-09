using MediatR;
using Microsoft.AspNetCore.Identity;
using FStore.Identity.Domain.Entities;
using FStore.Identity.Domain.Constants;

namespace FStore.Identity.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, string>
{
  private readonly UserManager<ApplicationUser> _userManager;

  public RegisterCommandHandler(UserManager<ApplicationUser> userManager)
  {
    _userManager = userManager;
  }
  public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
  {
    var user = new ApplicationUser
    {
      UserName = request.Email,
      Email = request.Email,
      FirstName = request.FirstName,
      LastName = request.LastName
    };

    var result = await _userManager.CreateAsync(user, request.Password);


    if (!result.Succeeded)
    {
      throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    var roleResult = await _userManager.AddToRoleAsync(user, UserRoles.Customer);
    if (!roleResult.Succeeded)
    {
      await _userManager.DeleteAsync(user);
      throw new Exception(string.Join(", ", roleResult.Errors.Select(e => e.Description)));
    }

    return user.Id;
  }
}