using FStore.Order.Application.Features.Orders.DTOs;
using MediatR;

namespace FStore.Order.Application.Features.Orders.Queries.GetOrdersByUser;

public record GetOrdersByUserQuery(Guid UserId) : IRequest<List<OrderDto>>;