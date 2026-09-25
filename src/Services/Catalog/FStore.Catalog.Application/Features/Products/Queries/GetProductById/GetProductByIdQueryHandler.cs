using System.Data.Common;
using FStore.Catalog.Application.Features.Products.DTOs;
using FStore.Catalog.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FStore.Catalog.Application.Features.Categories.Queries.GetCategoryById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
  private readonly CatalogDbContext _context;

  public GetProductByIdQueryHandler(CatalogDbContext context)
  {
    _context = context;
  }
  public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken ct)
  {
    return await _context.Products
      .AsNoTracking()
      .Where(p => p.Id == request.Id)
      .Select(p => new ProductDto(
        p.Id,
        p.Name,
        p.Description,
        p.Price.Amount,
        p.Price.Currency,
        p.StockQuantity,
        p.CategoryId,
        p.Category != null ? p.Category.Name : string.Empty
      )).FirstOrDefaultAsync(ct);
  }
}