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
    var query = _context.Products.AsNoTracking();

    if (!string.IsNullOrWhiteSpace(request.SearchTerm))
    {
      var term = request.SearchTerm.ToLower();
      query = query.Where(p =>
        p.Name.ToLower().Contains(term) ||
        p.Description.ToLower().Contains(term)
      );
    }
    return await query
      .Select(p => new ProductDto
      (
        p.Id, p.Name, p.Description,
        p.Price.Amount, p.Price.Currency,
        p.StockQuantity, p.CategoryId
      ))
      .ToListAsync(ct);
  }
}