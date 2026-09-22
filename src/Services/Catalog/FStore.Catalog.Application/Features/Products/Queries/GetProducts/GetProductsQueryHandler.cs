using FStore.Catalog.Application.Features.Products.DTOs;
using FStore.Catalog.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FStore.Catalog.Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
  private readonly CatalogDbContext _context;
  public GetProductsQueryHandler(CatalogDbContext context) => _context = context;
  public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken ct)
  {
    return await _context.Products
      .AsNoTracking()
      .Select(p => new ProductDto
      (
        p.Id, p.Name, p.Description,
        p.Price.Amount, p.Price.Currency,
        p.StockQuantity, p.CategoryId
      ))
      .ToListAsync(ct);
  }
}