using FluentAssertions;
using FStore.Catalog.Application.Features.Products.Commands.CreateProduct;
using FStore.Catalog.UnitTests.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace FStore.Catalog.UnitTests.Features.Products;

public class CreateProductCommandHandlerTests
{
  [Fact]
  public async Task HandleWithValidCommandReturnsNewProductId()
  {
    using var db = InMemoryDbContextFactory.Create();
    var categoryId = Guid.NewGuid();
    db.Categories.Add(new Domain.Entities.Category(categoryId, "Electronics"));
    await db.SaveChangesAsync();

    var handler = new CreateProductCommandHandler(db);
    var command = new CreateProductCommand(
      Name: "Laptop",
      Description: "Gaming Laptop",
      Price: 1499.99m,
      StockQuantity: 5,
      CategoryId: categoryId
    );

    var result = await handler.Handle(command, CancellationToken.None);

    result.Should().NotBe(Guid.Empty);
    var saved = await db.Products.FirstAsync();
    saved.Name.Should().Be("Laptop");
    saved.Price.Amount.Should().Be(1499.99m);
    saved.StockQuantity.Should().Be(5);
  }

  [Fact]
  public async Task HandleSavesProductToDatabase()
  {
    using var db = InMemoryDbContextFactory.Create();
    var handler = new CreateProductCommandHandler(db);
    var command = new CreateProductCommand(
      Name: "Test",
      Description: "Desc",
      Price: 9.99m,
      StockQuantity: 1,
      CategoryId: Guid.NewGuid()
    );

    await handler.Handle(command, CancellationToken.None);

    (await db.Products.CountAsync()).Should().Be(1);
  }
}