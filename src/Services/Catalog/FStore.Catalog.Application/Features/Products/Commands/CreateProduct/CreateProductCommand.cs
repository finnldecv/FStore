using MediatR;

namespace FStore.Catalog.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
  string Name,
  string Description,
  decimal Price,
  int StockQuantity,
  Guid CategoryId
) : IRequest<Guid>;