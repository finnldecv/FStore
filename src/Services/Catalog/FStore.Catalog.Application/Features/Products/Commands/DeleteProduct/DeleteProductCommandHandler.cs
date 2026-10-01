using FStore.Catalog.Domain.ValueObjects;
using FStore.Catalog.Infrastructure.Data;
using FStore.EventBus;
using FStore.EventBus.Events;
using MediatR;

namespace FStore.Catalog.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
  private readonly CatalogDbContext _context;
  private readonly IEventBus _eventBus;

  public DeleteProductCommandHandler(CatalogDbContext context, IEventBus eventBus)
  {
    _context = context;
    _eventBus = eventBus;
  }
  public async Task<bool> Handle(DeleteProductCommand request, CancellationToken ct)
  {
    var product = await _context.Products.FindAsync(new object[] { request.Id }, ct);
    if (product is null) return false;

    _context.Remove(product);
    await _context.SaveChangesAsync(ct);

    await _eventBus.PublishAsync(new ProductDeletedEvent
    {
      ProductId = product.Id
    }, ct);
    return true;
  }
}