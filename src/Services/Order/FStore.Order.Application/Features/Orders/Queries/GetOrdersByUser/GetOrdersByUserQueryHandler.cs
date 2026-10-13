using FStore.Order.Application.Features.Orders.DTOs;
using FStore.Order.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FStore.Order.Application.Features.Orders.Queries.GetOrdersByUser;

public class GetOrdersByUserQueryHandler
    : IRequestHandler<GetOrdersByUserQuery, List<OrderDto>>
{
  private readonly OrderDbContext _db;

  public GetOrdersByUserQueryHandler(OrderDbContext db) => _db = db;

  public async Task<List<OrderDto>> Handle(GetOrdersByUserQuery request, CancellationToken ct)
  {
    return await _db.Orders
        .AsNoTracking()
        .Where(o => o.UserId == request.UserId)
        .Include(o => o.Items)
        .OrderByDescending(o => o.CreatedAt)
        .Select(o => new OrderDto(
            o.Id,
            o.UserId,
            o.Status.ToString(),
            o.TotalAmount,
            o.ShippingAddress,
            o.Notes,
            o.CreatedAt,
            o.Items.Select(i => new OrderItemDto(
                i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.LineTotal)).ToList()))
        .ToListAsync(ct);
  }
}