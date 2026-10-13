using FStore.Order.Application.Features.Orders.DTOs;
using MediatR;

namespace FStore.Order.Application.Features.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto?>;