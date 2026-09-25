using FStore.Catalog.Application.Features.Categories.DTOs;
using FStore.Catalog.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FStore.Catalog.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
  private readonly CatalogDbContext _context;

  public GetCategoryByIdQueryHandler(CatalogDbContext context)
  {
    _context = context;
  }
  public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken ct)
  {
    return await _context.Categories
      .AsNoTracking()
      .Where(c => c.Id == request.Id)
      .Select(c => new CategoryDto(
        c.Id,
        c.Name,
        c.Products.Count
      )).FirstOrDefaultAsync(ct);
  }
}