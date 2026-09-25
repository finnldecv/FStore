using FStore.Catalog.Domain.Entities;
using FStore.Catalog.Infrastructure.Data;
using MediatR;

namespace FStore.Catalog.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
  private readonly CatalogDbContext _context;
  public CreateCategoryCommandHandler(CatalogDbContext context) => _context = context;
  public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken ct)
  {
    var category = new Category { Name = request.Name };
    _context.Categories.Add(category);
    await _context.SaveChangesAsync(ct);
    return category.Id;
  }
}
