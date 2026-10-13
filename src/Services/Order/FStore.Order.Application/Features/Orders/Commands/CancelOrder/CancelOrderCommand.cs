using MediatR;

namespace FStore.Order.Application.Features.Orders.Commands.CancelOrder;

public record CancelOrderCommand(Guid OrderId, string Reason) : IRequest<bool>;