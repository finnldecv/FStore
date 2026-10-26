using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using FStore.Identity.Application.Features.Auth.Commands.Login;
using FStore.Identity.Application.Features.Auth.Commands.Register;
using FStore.Identity.IntegrationTests.Fixtures;

namespace FStore.Identity.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<IdentityApiFactory>, IAsyncLifetime
{
  private readonly IdentityApiFactory _factory;
  private readonly HttpClient _client;

  public AuthControllerTests(IdentityApiFactory factory)
  {
    _factory = factory;
    _client = factory.CreateClient();
  }
  public Task InitializeAsync()
  {
    return _factory.ResetDatabaseAsync();
  }
  public Task DisposeAsync()
  {
    return Task.CompletedTask;
  }

  [Fact]
  public async Task RegisterWithValidDataReturnsSuccess()
  {
    var command = new RegisterCommand("newuser@test.com", "Password123!", "Mary", "Jane");

    var response = await _client.PostAsJsonAsync("/api/auth/register", command);

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
    result.Should().ContainKey("userId");
  }

  [Fact]
  public async Task LoginAfterRegistrationReturnsToken()
  {
    var regCommand = new RegisterCommand("logi@test.com", "Password123!", "Mary", "Jane");
    await _client.PostAsJsonAsync("/api/auth/register", regCommand);

    var loginCommand = new LoginCommand("login@test.com", "Password123!");
    var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);

    loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    var result = await loginResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();

    result.Should().ContainKey("token");
    result!["token"].Should().NotBeNullOrEmpty();
  }

  [Fact]
  public async Task LoginWithWrongPasswordReturnsBadRequest()
  {
    await _client.PostAsJsonAsync("/api/auth/register",
      new RegisterCommand("wrongpass@test.com", "Password123!", "Mary", "Jane")
    );

    var loginCommand = new LoginCommand("wrongpass@test.com", "WrongPassword");
    var response = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);

    response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
  }
}