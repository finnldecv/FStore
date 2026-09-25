using MediatR;

namespace FStore.Catalog.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
  Guid Id,
  string Name,
  string Description,
  decimal Price,
  int StockQuantity,
  Guid CategoryId) : IRequest<bool>;