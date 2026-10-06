using AwesomeAssertions;
using FStore.Basket.Application.Features.Baskets.Commands.DeleteBasket;
using FStore.Basket.Infrastructure.Data;
using FStore.Basket.Domain.Entities;
using NSubstitute;

namespace FStore.Basket.UnitTests.Features.Basket;

public class DeleteBasketCommandHandlerTests
{
  [Fact]
  public async Task HandleWithExistingBasketDeletesAndReturnsTrue()
  {
    var userId = Guid.NewGuid();
    var existing = new ShoppingCart { UserId = userId };

    var repository = Substitute.For<IBasketRepository>();
    repository.GetBasketAsync(userId).Returns(existing);
    var handler = new DeleteBasketCommandHandler(repository);

    var result = await handler.Handle(new DeleteBasketCommand(userId), CancellationToken.None);
    result.Should().BeTrue();
    await repository.Received(1).DeleteBasketAsync(userId);
  }

  [Fact]
  public async Task HandleWithMissingBasketReturnsFalse()
  {
    var repository = Substitute.For<IBasketRepository>();
    repository.GetBasketAsync(Arg.Any<Guid>()).Returns((ShoppingCart?)null);
    var handler = new DeleteBasketCommandHandler(repository);

    var result = await handler.Handle(new DeleteBasketCommand(Guid.NewGuid()), CancellationToken.None);
    result.Should().BeFalse();
    await repository.DidNotReceiveWithAnyArgs().DeleteBasketAsync(Arg.Any<Guid>());
  }
}