using FluentAssertions;
using FStore.Catalog.Application.Features.Products.Commands.DeleteProduct;
using FStore.Catalog.Domain.Entities;
using FStore.Catalog.Domain.ValueObjects;
using FStore.Catalog.UnitTests.TestHelpers;
using FStore.EventBus;
using FStore.EventBus.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace FStore.Catalog.UnitTests.Features.Products;

public class DeleteProductCommandHandlerTests
{
  [Fact]
  public async Task HandleWithExistingProductDeletesAndPublishesEvent()
  {
    using var db = InMemoryDbContextFactory.Create();
    var product = new Product
    {
      Name = "Laptop",
      Description = "X",
      Price = Money.Usd(1000m),
      StockQuantity = 1,
      CategoryId = Guid.NewGuid()
    };
    db.Products.Add(product);
    await db.SaveChangesAsync();

    var eventBus = Substitute.For<IEventBus>();
    var handler = new DeleteProductCommandHandler(db, eventBus);

    var result = await handler.Handle(
      new DeleteProductCommand(product.Id), CancellationToken.None
    );

    result.Should().BeTrue();
    (await db.Products.CountAsync()).Should().Be(0);
    await eventBus.Received(1).PublishAsync(
      Arg.Is<ProductDeletedEvent>(e => e.ProductId == product.Id),
      Arg.Any<CancellationToken>()
    );
  }

  [Fact]
  public async Task HandleWithMissingProductReturnsFalse()
  {
    using var db = InMemoryDbContextFactory.Create();
    var eventBus = Substitute.For<IEventBus>();
    var handler = new DeleteProductCommandHandler(db, eventBus);

    var result = await handler.Handle(
      new DeleteProductCommand(Guid.NewGuid()), CancellationToken.None
    );
    result.Should().BeFalse();
  }
}