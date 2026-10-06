using AwesomeAssertions;
using FStore.Catalog.Application.Features.Categories.Commands.UpdateCategory;
using FStore.Catalog.Domain.Entities;
using FStore.Catalog.UnitTests.TestHelpers;

namespace FStore.Catalog.UnitTests.Features.Categories;

public class UpdateCategoryCommandHandlerTests
{
  [Fact]
  public async Task HandleWhenCategoriesDoesNotExistsUpdatesReturnsFalse()
  {
    using var db = InMemoryDbContextFactory.Create();
    var handler = new UpdateCategoryCommandHandler(db);
    var categoryId = Guid.NewGuid();
    var command = new UpdateCategoryCommand(categoryId, Name: "Electronics");

    var result = await handler.Handle(command, CancellationToken.None);
    result.Should().BeFalse();
  }
  [Fact]
  public async Task HandleWithExistingCategoryUpdatesReturnsTrue()
  {
    using var db = InMemoryDbContextFactory.Create();
    var handler = new UpdateCategoryCommandHandler(db);
    
    var categoryId = Guid.NewGuid();
    var category = new Category(categoryId, "Electronics");
    db.Categories.Add(category);
    await db.SaveChangesAsync();

    var command = new UpdateCategoryCommand(categoryId, "X");

    var result = await handler.Handle(command, CancellationToken.None);
    result.Should().BeTrue();
  }
}