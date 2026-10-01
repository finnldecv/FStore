using FStore.Catalog.Domain.ValueObjects;
using FStore.Catalog.Infrastructure.Data;
using FStore.EventBus;
using FStore.EventBus.Events;
using MediatR;

namespace FStore.Catalog.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
  private readonly CatalogDbContext _context;
  private readonly IEventBus _eventBus;

  public UpdateProductCommandHandler(CatalogDbContext context, IEventBus eventBus)
  {
    _context = context;
    _eventBus = eventBus;
  }
  public async Task<bool> Handle(UpdateProductCommand request, CancellationToken ct)
  {
    var product = await _context.Products.FindAsync(new object[] { request.Id }, ct);

    if (product is null) return false;

    var oldPrice = product.Price.Amount;

    product.Name = request.Name;
    product.Description = request.Description;
    product.Price = Money.Usd(request.Price);
    product.StockQuantity = request.StockQuantity;
    product.CategoryId = request.CategoryId;
    product.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync(ct);

    if (oldPrice != request.Price)
    {
      await _eventBus.PublishAsync(new ProductPriceChangedEvent
      {
        ProductId = product.Id,
        ProductName = product.Name,
        OldPrice = oldPrice,
        NewPrice = request.Price
      }, ct);
    }
    return true;
  }
}