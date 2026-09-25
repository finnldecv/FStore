using FStore.Catalog.Application.Features.Categories.DTOs;
using FStore.Catalog.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FStore.Catalog.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
  private readonly CatalogDbContext _context;
  public GetCategoriesQueryHandler(CatalogDbContext context) => _context = context;
  public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct)
  {
    return await _context.Categories
      .AsNoTracking()
      .Select(c => new CategoryDto(
        c.Id,
        c.Name,
        c.Products.Count
      ))
      .ToListAsync(ct);
  }
}