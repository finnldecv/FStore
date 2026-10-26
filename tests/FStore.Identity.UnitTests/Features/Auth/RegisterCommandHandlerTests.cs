using AwesomeAssertions;
using FStore.Identity.Application.Features.Auth.Commands.Register;
using FStore.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace FStore.Identity.UnitTests.Features.Auth;

public class RegisterCommandHandlerTests
{
  [Fact]
  public async Task HandleWithValidDataReturnsUserId()
  {
    var userManager = Substitute.For<UserManager<ApplicationUser>>(
      Substitute.For<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null
    );

    userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);

    var handler = new RegisterCommandHandler(userManager);
    var command = new RegisterCommand("test@test.com", "Password123!", "John", "Cena");

    var result = await handler.Handle(command, CancellationToken.None);

    result.Should().NotBeNullOrEmpty();
    await userManager.Received(1).CreateAsync(
      Arg.Is<ApplicationUser>(u => u.Email == "test@test.com"), "Password123!"
    );
  }

  [Fact]
  public async Task HandleWhenIdentityFailsThrowsException()
  {
    var userManager = Substitute.For<UserManager<ApplicationUser>>(
      Substitute.For<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null
    );

    var failedResult = IdentityResult.Failed(new IdentityError { Description = "Password too weak" });
    userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(failedResult);

    var handler = new RegisterCommandHandler(userManager);
    var command = new RegisterCommand("test@test.com", "weak", "John", "Cena");

    var act = () => handler.Handle(command, CancellationToken.None);

    await act.Should().ThrowAsync<Exception>().WithMessage("*Password too weak");
  }
}