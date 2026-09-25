using FStore.Catalog.Domain.ValueObjects;
using FStore.Catalog.Infrastructure.Data;
using MediatR;

namespace FStore.Catalog.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
  private readonly CatalogDbContext _context;

  public UpdateProductCommandHandler(CatalogDbContext context)
  {
    _context = context;
  }
  public async Task<bool> Handle(UpdateProductCommand request, CancellationToken ct)
  {
    var product = await _context.Products.FindAsync(new object[] { request.Id }, ct);

    if (product is null) return false;

    product.Name = request.Name;
    product.Description = request.Description;
    product.Price = Money.Usd(request.Price);
    product.StockQuantity = request.StockQuantity;
    product.CategoryId = request.CategoryId;
    product.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync(ct);
    return true;
  }
}