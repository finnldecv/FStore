using FStore.Catalog.Application.Features.Products.DTOs;
using FStore.Catalog.Infrastructure.Data;
using FStore.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FStore.Catalog.Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
  private readonly CatalogDbContext _context;
  public GetProductsQueryHandler(CatalogDbContext context) => _context = context;
  public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken ct)
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

    var totalCount = await query.CountAsync(ct);

    var items = await query
    .OrderBy(p => p.Name)
    .Skip((request.PageNumber - 1) * request.PageSize)
    .Take(request.PageSize)
    .Select(p => new ProductDto
      (
        p.Id, p.Name, p.Description,
        p.Price.Amount, p.Price.Currency,
        p.StockQuantity, p.CategoryId
      ))
      .ToListAsync(ct);

    return new PagedResult<ProductDto>
    {
      Items = items,
      TotalCount = totalCount,
      PageNumber = request.PageNumber,
      PageSize = request.PageSize
    };
  }
}