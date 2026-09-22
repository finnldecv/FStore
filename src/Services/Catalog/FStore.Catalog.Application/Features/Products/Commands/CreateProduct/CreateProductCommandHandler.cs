using FStore.Catalog.Domain.Entities;
using FStore.Catalog.Domain.ValueObjects;
using FStore.Catalog.Infrastructure.Data;
using MediatR;

namespace FStore.Catalog.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
  private readonly CatalogDbContext _context;
  public CreateProductCommandHandler(CatalogDbContext context) => _context = context;
  public async Task<Guid> Handle(CreateProductCommand request, CancellationToken ct)
  {
    var product = new Product
    {
      Name = request.Name,
      Description = request.Description,
      Price = Money.Usd(request.Price),
      StockQuantity = request.StockQuantity,
      CategoryId = request.CategoryId
    };

    _context.Products.Add(product);
    await _context.SaveChangesAsync(ct);
    return product.Id;
  }
}