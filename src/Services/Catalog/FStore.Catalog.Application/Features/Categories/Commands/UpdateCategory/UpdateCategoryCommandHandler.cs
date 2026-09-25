using FStore.Catalog.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FStore.Catalog.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, bool>
{
  private readonly CatalogDbContext _context;
  public UpdateCategoryCommandHandler(CatalogDbContext context)
  {
    _context = context;
  }
  public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken ct)
  {
    var category = await _context.Categories.FindAsync( new object [] {request.Id}, ct);

    if(category is null) return false;

    category.Name = request.Name;
    category.UpdatedAt = DateTime.UtcNow;
    await _context.SaveChangesAsync(ct);
    return true;
  }
}