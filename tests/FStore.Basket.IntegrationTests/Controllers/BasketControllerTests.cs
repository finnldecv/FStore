using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using FStore.Basket.Application.Features.Baskets.Commands.CreateBasket;
using FStore.Basket.Application.Features.Baskets.Commands.UpdateBasket;
using FStore.Basket.Domain.Entities;
using FStore.Basket.IntegrationTests.Fixtures;

namespace FStore.Basket.IntegrationTests.Controllers;

public class BasketControllerTests : IClassFixture<BasketApiFactory>
{
  private readonly HttpClient _client;

  public BasketControllerTests(BasketApiFactory factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task CreateThenGetReturnsBasket()
  {
    var userId = Guid.NewGuid();
    var command = new CreateBasketCommand(userId, new List<BasketItemDto>
    {
      new(Guid.NewGuid(), "Laptop", 1499.99m ,2)
    });

    var createReponse = await _client.PostAsJsonAsync("/api/basket", command);
    createReponse.StatusCode.Should().Be(HttpStatusCode.OK);

    var getResponse = await _client.GetAsync($"/api/basket/{userId}");
    getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

    var basket = await getResponse.Content.ReadFromJsonAsync<ShoppingCart>();
    basket.Should().NotBeNull();
    basket!.UserId.Should().Be(userId);
    basket.Items.Should().HaveCount(1);
    basket.Items[0].ProductName.Should().Be("Laptop");
  }

  [Fact]
  public async Task GetWithUnknownUserReturnsNotFound()
  {
    var response = await _client.GetAsync($"/api/basket/{Guid.NewGuid()}");
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task UpdateWithExistingBasketReturnsNoContent()
  {
    var userId = Guid.NewGuid();

    await _client.PostAsJsonAsync("/api/basket",
    new CreateBasketCommand(userId, new List<BasketItemDto>
    {
      new(Guid.NewGuid(), "Laptop", 1499.99m, 1)
    }));

    var updateCommand = new UpdateBasketCommand(userId, new List<UpdateBasketItemDto>
      {
        new(Guid.NewGuid(), "Laptop", 1499.99m, 5)
      });

    var updateResponse = await _client.PutAsJsonAsync(
      $"/api/basket/{userId}", updateCommand
    );
    updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

    var getResponse = await _client.GetAsync($"/api/basket/{userId}");
    var basket = await getResponse.Content.ReadFromJsonAsync<ShoppingCart>();
    basket!.Items[0].Quantity.Should().Be(5);
  }

  [Fact]
  public async Task UpdateWithMissingBasketReturnsNotFound()
  {
    var userId = Guid.NewGuid();
    var command = new UpdateBasketCommand(userId, new List<UpdateBasketItemDto>());

    var response = await _client.PutAsJsonAsync($"/api/basket/{userId}", command);
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task DeleteWithExistingBasketReturnsNoContent()
  {
    var userId = Guid.NewGuid();
    await _client.PostAsJsonAsync("/api/basket",
      new CreateBasketCommand(userId, new List<BasketItemDto>
      {
        new(Guid.NewGuid(), "Laptop", 1499.99m, 1)
      })
    );

    var deleteResponse = await _client.DeleteAsync($"/api/basket/{userId}");
    deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

    var getResponse = await _client.GetAsync($"/api/basket/{userId}");
    getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task DeleteWithMissingBasketReturnsNoContent()
  {
    var userId = Guid.NewGuid();

    var deleteResponse = await _client.DeleteAsync($"/api/basket/{userId}");
    deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }
}