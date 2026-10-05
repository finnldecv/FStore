using System.Net;
using AwesomeAssertions;
using FStore.Catalog.Infrastructure.Data;
using FStore.Catalog.IntegrationTests.Fixtures;

namespace FStore.Catalog.IntegrationTests.Controllers;

public class HealthTests : IClassFixture<CatalogApiFactory>
{
  private readonly HttpClient _client;
  public HealthTests(CatalogApiFactory factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task ApiStartsAndResponds()
  {
    var response = await _client.GetAsync("/api/products");
    response.StatusCode.Should().Be(HttpStatusCode.OK);
  }
}