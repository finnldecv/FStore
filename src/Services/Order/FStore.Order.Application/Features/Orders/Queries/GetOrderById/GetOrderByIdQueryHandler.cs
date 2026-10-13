using FStore.Order.Application.Features.Orders.DTOs;
using FStore.Order.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FStore.Order.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
  private readonly OrderDbContext _db;

  public GetOrderByIdQueryHandler(OrderDbContext db) => _db = db;

  public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken ct)
  {
    var order = await _db.Orders
        .AsNoTracking()
        .Include(o => o.Items)
        .FirstOrDefaultAsync(o => o.Id == request.Id, ct);

    if (order is null) return null;

    return new OrderDto(
        order.Id,
        order.UserId,
        order.Status.ToString(),
        order.TotalAmount,
        order.ShippingAddress,
        order.Notes,
        order.CreatedAt,
        order.Items.Select(i => new OrderItemDto(
            i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.LineTotal)).ToList());
  }
}