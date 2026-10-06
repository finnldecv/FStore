using AwesomeAssertions;
using FStore.Basket.Application.Features.Baskets.Queries.GetBasket;
using FStore.Basket.Domain.Entities;
using FStore.Basket.Infrastructure.Data;
using NSubstitute;

namespace FStore.Basket.UnitTests.Features.Basket;

public class GetBasketQueryHandlerTests
{
  [Fact]
  public async Task HandleWhenBasketExistsReturnsBasket()
  {
    var userId = Guid.NewGuid();
    var expected = new ShoppingCart
    {
      UserId = userId,
      Items = new List<ShoppingCartItem>
      {
        new()
        {
          ProductId = Guid.NewGuid(),
          ProductName = "Laptop",
          Price = 1499.99m, Quantity = 1
        }
      }
    };

    var repository = Substitute.For<IBasketRepository>();
    repository.GetBasketAsync(userId).Returns(expected);
    var handler = new GetBasketQueryHandler(repository);
    var result = await handler.Handle(new GetBasketQuery(userId), CancellationToken.None);
    result.Should().NotBeNull();
    result!.UserId.Should().Be(userId);
    result.Items.Should().HaveCount(1);
  }

  [Fact]
  public async Task HandleWhenBasketMissingReturnsNull()
  {
    var repository = Substitute.For<IBasketRepository>();
    var userId = Guid.NewGuid();
    var handler = new GetBasketQueryHandler(repository);
    var result = await handler.Handle(new GetBasketQuery(userId), CancellationToken.None);
    result.Should().BeNull();
  }
}