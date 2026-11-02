using FStore.Catalog.Application.Features.Products.DTOs;
using FStore.Common.Models;
using MediatR;

namespace FStore.Catalog.Application.Features.Products.Queries.GetProducts;

public record GetProductsQuery(
  string? SearchTerm = null,
  Guid? CategoryId = null,
  decimal? MinPrice = null,
  decimal? MaxPrice = null,
  int PageNumber = 1,
  int PageSize = 12
) : IRequest<PagedResult<ProductDto>>;