namespace FStore.Catalog.Application.Features.Products.DTOs;

public record ProductDto(
  Guid Id,
  string name,
  string Description,
  decimal Price,
  string Currency,
  int StockQuantity,
  Guid CategoryId
);