using AwesomeAssertions;
using FStore.Basket.Application.Features.Baskets.Commands.CreateBasket;
using FStore.Basket.Domain.Entities;
using FStore.Basket.Infrastructure.Data;
using NSubstitute;

namespace FStore.Basket.UnitTests.Features.Basket;

public class CreateBasketCommandHandlerTests
{
  [Fact]
  public async Task HandleWithValidCommandSavesBasketToRepository()
  {
    var repository = Substitute.For<IBasketRepository>();
    var handler = new CreateBasketCommandHandler(repository);
    var userId = Guid.NewGuid();
    var command = new CreateBasketCommand(userId, new List<BasketItemDto>
    {
      new(Guid.NewGuid(),"Laptop", 1499.99m, 2)
    });

    var result = await handler.Handle(command, CancellationToken.None);
    result.Should().BeTrue();
    await repository.Received(1).SaveBasketAsync(
      Arg.Is<ShoppingCart>(c =>
        c.UserId == userId &&
        c.Items.Count == 1 &&
        c.Items[0].ProductName == "Laptop" &&
        c.Items[0].Quantity == 2)
    );
  }

  [Fact]
  public async Task HandleWithMultipleItemsSavesAllItems()
  {
    var repository = Substitute.For<IBasketRepository>();
    var handler = new CreateBasketCommandHandler(repository);
    var userId = Guid.NewGuid();
    var command = new CreateBasketCommand(userId, new List<BasketItemDto>
    {
      new(Guid.NewGuid(), "Laptop", 1499.99m, 1),
      new(Guid.NewGuid(), "Mouse", 29.99m, 2),
      new(Guid.NewGuid(), "Keyboard", 89.99m, 1)
    });

    await handler.Handle(command, CancellationToken.None);

    await repository.Received(1).SaveBasketAsync(
      Arg.Is<ShoppingCart>(c => c.Items.Count == 3)
    );
  }
}