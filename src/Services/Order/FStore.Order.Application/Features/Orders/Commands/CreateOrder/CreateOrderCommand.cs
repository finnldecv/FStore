using FStore.Order.Application.Features.Orders.DTOs;
using MediatR;

namespace FStore.Order.Application.Features.Orders.Commands.CreateOrder;

public record CreateOrderCommand(
  Guid UserId,
  string ShippingAddress,
  string? Notes,
  List<CreateOrderItemDto> Items
) : IRequest<OrderDto>;

public record CreateOrderItemDto(
  Guid ProductId,
  string ProductName,
  decimal UnitPrice,
  int Quantity
);