using AwesomeAssertions;
using FStore.Catalog.Application.Features.Categories.Commands.DeleteCategory;
using FStore.Catalog.Domain.Entities;
using FStore.Catalog.UnitTests.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace FStore.Catalog.UnitTests.Features.Categories;

public class DeleteProductCommandHandlerTests
{
  [Fact]
  public async Task HandleWithExistingCategoryDeletesReturnsTrue()
  {
    using var db = InMemoryDbContextFactory.Create();
    var handler = new DeleteCategoryCommandHandler(db);
    var categoryId = Guid.NewGuid();
    Category category = new Category(categoryId, "Electronics");
    var command = new DeleteCategoryCommand(categoryId);

    var result = await handler.Handle(command, CancellationToken.None);
    result.Should().BeTrue();
  }

  [Fact]
  public async Task HandleWhenCategoryDoesNotExistsDeletesReturnsFalse()
  {
    using var db = InMemoryDbContextFactory.Create();
    var handler = new DeleteCategoryCommandHandler(db);
    var categoryId = Guid.NewGuid();
    var command = new DeleteCategoryCommand(categoryId);

    var result = await handler.Handle(command, CancellationToken.None);
    result.Should().BeFalse();
  }
}