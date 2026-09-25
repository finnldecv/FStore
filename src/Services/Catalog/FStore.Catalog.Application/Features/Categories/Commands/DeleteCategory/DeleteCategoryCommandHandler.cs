using FStore.Catalog.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FStore.Catalog.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
  private readonly CatalogDbContext _context;
  public DeleteCategoryCommandHandler(CatalogDbContext context) => _context = context;
  public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken ct)
  {
    var category = await _context.Categories.FindAsync(new object[] { request.Id }, ct);
    if (category is null) return false;

    var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == request.Id, ct);
    if (hasProducts)
    {
      throw new InvalidOperationException("Cannot delete a category that still has products.");
    }

    _context.Categories.Remove(category);
    await _context.SaveChangesAsync(ct);
    return true;
  }
}