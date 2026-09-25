using FStore.Catalog.Domain.ValueObjects;
using FStore.Catalog.Infrastructure.Data;
using MediatR;

namespace FStore.Catalog.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
  private readonly CatalogDbContext _context;

  public DeleteProductCommandHandler(CatalogDbContext context)
  {
    _context = context;
  }
  public async Task<bool> Handle(DeleteProductCommand request, CancellationToken ct)
  {
    var product = await _context.Products.FindAsync(new object[] { request.Id}, ct);

    if (product is null) return false;

    _context.Remove(product);
    await _context.SaveChangesAsync();
    return true;
  }
}