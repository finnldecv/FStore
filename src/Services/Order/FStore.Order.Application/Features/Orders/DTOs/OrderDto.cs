namespace FStore.Order.Application.Features.Orders.DTOs;

public record OrderDto(
    Guid Id,
    Guid UserId,
    string Status,
    decimal TotalAmount,
    string ShippingAddress,
    string? Notes,
    DateTime CreatedAt,
    List<OrderItemDto> Items);

public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);