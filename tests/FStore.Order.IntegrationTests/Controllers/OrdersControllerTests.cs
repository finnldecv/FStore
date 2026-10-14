using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using FStore.Order.Application.Features.Orders.Commands.CreateOrder;
using FStore.Order.Application.Features.Orders.DTOs;
using FStore.Order.IntegrationTests.Fixtures;

namespace FStore.Order.IntegrationTests.Controllers;

public class OrdersControllerTests : IClassFixture<OrderApiFactory>, IAsyncLifetime
{
  private readonly HttpClient _client;
  private readonly OrderApiFactory _factory;

  public OrdersControllerTests(OrderApiFactory factory)
  {
    _factory = factory;
    _client = factory.CreateClient();
  }

  public Task InitializeAsync() => _factory.ResetDatabaseAsync();
  public Task DisposeAsync() => Task.CompletedTask;

  [Fact]
  public async Task GetOrdersByUserWithNoOrdersReturnsEmptyList()
  {
    var response = await _client.GetAsync($"/api/orders/user/{Guid.NewGuid()}");

    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>();
    orders.Should().NotBeNull();
    orders!.Should().BeEmpty();
  }

  [Fact]
  public async Task CreateThenGetByIdReturnsOrder()
  {
    var userId = Guid.NewGuid();
    var command = new CreateOrderCommand(
        userId,
        "123 Main St",
        "Leave at door",
        new List<CreateOrderItemDto>
        {
                new(Guid.NewGuid(), "Laptop", 1499.99m, 1)
        });

    var createResponse = await _client.PostAsJsonAsync("/api/orders", command);
    createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

    var created = await createResponse.Content.ReadFromJsonAsync<OrderDto>();
    created.Should().NotBeNull();
    created!.Id.Should().NotBe(Guid.Empty);

    var getResponse = await _client.GetAsync($"/api/orders/{created.Id}");
    getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

    var fetched = await getResponse.Content.ReadFromJsonAsync<OrderDto>();
    fetched!.UserId.Should().Be(userId);
    fetched.Status.Should().Be("Pending");
    fetched.TotalAmount.Should().Be(1499.99m);
  }

  [Fact]
  public async Task CreateThenGetByUserReturnsOrder()
  {
    var userId = Guid.NewGuid();
    var command = new CreateOrderCommand(
        userId, "123 Main St", null,
        new List<CreateOrderItemDto> { new(Guid.NewGuid(), "Item", 50m, 2) });

    await _client.PostAsJsonAsync("/api/orders", command);

    var response = await _client.GetAsync($"/api/orders/user/{userId}");
    var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>();

    orders.Should().HaveCount(1);
    orders![0].UserId.Should().Be(userId);
  }

  [Fact]
  public async Task CancelWithExistingOrderReturnsNoContent()
  {
    var command = new CreateOrderCommand(
        Guid.NewGuid(), "123 Main St", null,
        new List<CreateOrderItemDto> { new(Guid.NewGuid(), "Item", 100m, 1) });

    var createResponse = await _client.PostAsJsonAsync("/api/orders", command);
    var created = await createResponse.Content.ReadFromJsonAsync<OrderDto>();

    var cancelRequest = new HttpRequestMessage
    {
      Method = HttpMethod.Delete,
      RequestUri = new Uri($"/api/orders/{created!.Id}", UriKind.Relative),
      Content = JsonContent.Create(new { reason = "Changed mind" })
    };

    var cancelResponse = await _client.SendAsync(cancelRequest);

    cancelResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

    var getResponse = await _client.GetAsync($"/api/orders/{created.Id}");
    var fetched = await getResponse.Content.ReadFromJsonAsync<OrderDto>();
    fetched!.Status.Should().Be("Cancelled");
  }

  [Fact]
  public async Task GetById_WithUnknownId_ReturnsNotFound()
  {
    var response = await _client.GetAsync($"/api/orders/{Guid.NewGuid()}");
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task Create_WithEmptyItems_ReturnsBadRequest()
  {
    var invalid = new CreateOrderCommand(
        Guid.NewGuid(), "Address", null, new List<CreateOrderItemDto>());

    var response = await _client.PostAsJsonAsync("/api/orders", invalid);

    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
  }
}