using FStore.Catalog.Application.Features.Categories.DTOs;
using FStore.Catalog.Domain.Entities;
using FStore.Catalog.Infrastructure.Data;
using MediatR;

namespace FStore.Catalog.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
  private readonly CatalogDbContext _context;
  public CreateCategoryCommandHandler(CatalogDbContext context) => _context = context;
  public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken ct)
  {
    var category = new Category(request.Name);
    _context.Categories.Add(category);
    await _context.SaveChangesAsync(ct);

    return new CategoryDto(category.Id, category.Name, 0);
  }
}
