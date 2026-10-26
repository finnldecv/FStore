using AwesomeAssertions;
using FStore.Identity.Application.Features.Auth.Commands.Login;
using FStore.Identity.Application.Services;
using FStore.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace FStore.Identity.UnitTests.Features.Auth;

public class LoginCommandHandlerTests
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly ITokenService _tokenService;
  private readonly LoginCommandHandler _handler;

  public LoginCommandHandlerTests()
  {
    _userManager = Substitute.For<UserManager<ApplicationUser>>(
      Substitute.For<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null
    );
    _tokenService = Substitute.For<ITokenService>();
    _handler = new LoginCommandHandler(_userManager, _tokenService);
  }
  [Fact]
  public async Task HandleWithValidCredentialsReturnsToken()
  {
    var user = new ApplicationUser { Email = "test@test.com" };

    _userManager.FindByEmailAsync("test@test.com").Returns(user);
    _userManager.CheckPasswordAsync(user, "Password123!").Returns(true);
    _tokenService.GenerateToken(user).Returns("fake-jwt-token");

    var result = await _handler.Handle(new LoginCommand("test@test.com", "Password123!"), CancellationToken.None);

    result.Should().Be("fake-jwt-token");
  }

  [Fact]
  public async Task HandleWithWrongPasswordThrowsException()
  {
    var user = new ApplicationUser { Email = "test@test.com" };

    _userManager.FindByEmailAsync("test@test.com").Returns(user);
    _userManager.CheckPasswordAsync(user, "WrongPassword").Returns(false);

    var act = () => _handler.Handle(new LoginCommand("test@test.com", "WrongPassword"), CancellationToken.None);

    await act.Should().ThrowAsync<Exception>().WithMessage("*Invalid email or password");
  }
}