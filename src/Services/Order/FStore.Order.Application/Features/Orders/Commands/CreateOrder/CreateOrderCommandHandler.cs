using FStore.Order.Application.Features.Orders.DTOs;
using FStore.Order.Domain.Entities;
using FStore.Order.Infrastructure.Data;
using FStore.EventBus;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FStore.EventBus.Events;

namespace FStore.Order.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
  private readonly OrderDbContext _db;
  private readonly IEventBus _eventBus;

  public CreateOrderCommandHandler(OrderDbContext db, IEventBus eventBus)
  {
    _db = db;
    _eventBus = eventBus;
  }

  public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken ct)
  {
    var order = new OrderEntity
    {
      UserId = request.UserId,
      ShippingAddress = request.ShippingAddress,
      Notes = request.Notes
    };

    foreach (var item in request.Items)
    {
      order.AddItem(item.ProductId, item.ProductName, item.UnitPrice, item.Quantity);
    }

    _db.Orders.Add(order);
    await _db.SaveChangesAsync(ct);

    // Publish so Inventory can reserve stock
    // (Inventory service doesn't exist yet — the event just sits in RabbitMQ)
    await _eventBus.PublishAsync(new OrderCreatedEvent
    {
      OrderId = order.Id,
      UserId = order.UserId,
      TotalAmount = order.TotalAmount,
      Items = order.Items.Select(i => new OrderCreatedItem
      {
        ProductId = i.ProductId,
        Quantity = i.Quantity
      }).ToList()
    }, ct);

    return MapToDto(order);
  }

  private static OrderDto MapToDto(OrderEntity o) => new(
      o.Id,
      o.UserId,
      o.Status.ToString(),
      o.TotalAmount,
      o.ShippingAddress,
      o.Notes,
      o.CreatedAt,
      o.Items.Select(i => new OrderItemDto(
          i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.LineTotal)).ToList());
}