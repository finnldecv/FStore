using AwesomeAssertions;
using FStore.Catalog.Application.Features.Categories.Commands.CreateCategory;
using FStore.Catalog.UnitTests.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace FStore.Catalog.UnitTests.Features.Categories;

public class CreateCategoryCommandHandlerTests
{
  [Fact]
  public async Task HandleWithValidCommandCreateReturnsNewCategoryId()
  {
    using var db = InMemoryDbContextFactory.Create();
    var handler = new CreateCategoryCommandHandler(db);
    var command = new CreateCategoryCommand(
      Name: "Electronics"
    );

    var result = await handler.Handle(command, CancellationToken.None);

    result.Should().NotBe(Guid.Empty);
    var saved = await db.Categories.FirstAsync();
    saved.Name.Should().Be("Electronics");
  }
}