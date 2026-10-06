using AwesomeAssertions;
using FStore.Basket.Application.Features.Baskets.Commands.UpdateBasket;
using FStore.Basket.Domain.Entities;
using FStore.Basket.Infrastructure.Data;
using NSubstitute;

namespace FStore.Basket.UnitTests.Features.Basket;

public class UpdateBasketCommandHandlerTests
{
  [Fact]
  public async Task HandleWithExistingBasketUpdatsAndSaves()
  {
    var userId = Guid.NewGuid();
    var existing = new ShoppingCart { UserId = userId, Items = new List<ShoppingCartItem>() };

    var repository = Substitute.For<IBasketRepository>();
    repository.GetBasketAsync(userId).Returns(existing);
    var handler = new UpdateBasketCommandHandler(repository);

    var command = new UpdateBasketCommand(userId, new List<UpdateBasketItemDto>
    {
      new(Guid.NewGuid(), "Laptop", 1499.99m, 3),
      new(Guid.NewGuid(), "Mouse", 29.99m, 1)
    });

    var result = await handler.Handle(command, CancellationToken.None);

    result.Should().BeTrue();
    existing.Items.Should().HaveCount(2);
    existing.Items[0].Quantity.Should().Be(3);
    await repository.Received(1).SaveBasketAsync(existing);
  }

  [Fact]
  public async Task HandleWithMissingBasketReturnsFalse()
  {
    var repository = Substitute.For<IBasketRepository>();
    repository.GetBasketAsync(Arg.Any<Guid>()).Returns((ShoppingCart?)null);
    var handler = new UpdateBasketCommandHandler(repository);

    var result = await handler.Handle(
      new UpdateBasketCommand(
        Guid.NewGuid(),
        new List<UpdateBasketItemDto>()
      ),
      CancellationToken.None
    );
    result.Should().BeFalse();
    await repository.DidNotReceiveWithAnyArgs().SaveBasketAsync(Arg.Any<ShoppingCart>());
  }
}