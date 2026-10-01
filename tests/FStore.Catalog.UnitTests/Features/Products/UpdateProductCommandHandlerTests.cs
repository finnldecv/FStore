using FluentAssertions;
using FStore.Catalog.Application.Features.Products.Commands.UpdateProduct;
using FStore.Catalog.Domain.Entities;
using FStore.Catalog.Domain.ValueObjects;
using FStore.Catalog.UnitTests.TestHelpers;
using FStore.EventBus;
using FStore.EventBus.Events;
using NSubstitute;

namespace FStore.Catalog.UnitTests.Features.Products;

public class UpdateProductCommandHandlerTests
{
  [Fact]
  public async Task HandleWhenProductDoesNotExistReturnsFalse()
  {
    using var db = InMemoryDbContextFactory.Create();
    var eventBus = Substitute.For<IEventBus>();
    var handler = new UpdateProductCommandHandler(db, eventBus);

    var result = await handler.Handle(new UpdateProductCommand(
      Guid.NewGuid(), "X", "Y", 1m, 1, Guid.NewGuid()
    ), CancellationToken.None);

    result.Should().BeFalse();
    await eventBus.DidNotReceiveWithAnyArgs().PublishAsync(Arg.Any<object>());
  }

  [Fact]
  public async Task HandleWhenPriceChangesPublishesProductPriceChangedEvent()
  {
    using var db = InMemoryDbContextFactory.Create();
    var product = new Product
    {
      Name = "Laptop",
      Description = "Gaming",
      Price = Money.Usd(1000m),
      StockQuantity = 5,
      CategoryId = Guid.NewGuid()
    };
    db.Products.Add(product);
    await db.SaveChangesAsync();

    var eventBus = Substitute.For<IEventBus>();
    var handler = new UpdateProductCommandHandler(db, eventBus);

    await handler.Handle(new UpdateProductCommand(
      product.Id, "Laptop", "Gaming", 1500m, 5, product.CategoryId),
        CancellationToken.None);

    await eventBus.Received(1).PublishAsync(
      Arg.Is<ProductPriceChangedEvent>(e =>
                e.ProductId == product.Id &&
                e.OldPrice == 1000m &&
                e.NewPrice == 1500m),
            Arg.Any<CancellationToken>()
    );
  }

  [Fact]
  public async Task HandleWhenPriceUnchangedDoesNotPublishEvent()
  {
    using var db = InMemoryDbContextFactory.Create();
    var product = new Product
    {
      Name = "Laptop",
      Description = "Gaming",
      Price = Money.Usd(1000m),
      StockQuantity = 5,
      CategoryId = Guid.NewGuid()
    };
    db.Products.Add(product);
    await db.SaveChangesAsync();

    var eventBus = Substitute.For<IEventBus>();
    var handler = new UpdateProductCommandHandler(db, eventBus);

    await handler.Handle(new UpdateProductCommand(
      product.Id, "Laptop Renamed", "Gaming", 1000m, 5, product.CategoryId),
        CancellationToken.None);

    await eventBus.DidNotReceiveWithAnyArgs()
      .PublishAsync(Arg.Any<ProductPriceChangedEvent>(), Arg.Any<CancellationToken>());
  }
}