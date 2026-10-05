using System.Net;
using AwesomeAssertions;
using FStore.Basket.Infrastructure.Data;
using FStore.Basket.IntegrationTests.Fixtures;

namespace FStore.Basket.IntegrationTests.Controllers;

public class HealthTests : IClassFixture<BasketApiFactory>
{
  private readonly HttpClient _client;
  public HealthTests(BasketApiFactory factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task ApiStartsAndResponds()
  {
    var response = await _client.GetAsync($"/api/baskets/{Guid.NewGuid()}");
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }
}