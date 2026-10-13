using FStore.Order.Infrastructure.Data;
using FStore.EventBus;
using FStore.EventBus.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FStore.Order.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
{
    private readonly OrderDbContext _db;
    private readonly IEventBus _eventBus;

    public CancelOrderCommandHandler(OrderDbContext db, IEventBus eventBus)
    {
        _db = db;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken ct)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null) return false;

        order.MarkAsCancelled(request.Reason);
        await _db.SaveChangesAsync(ct);

        await _eventBus.PublishAsync(new OrderCancelledEvent
        {
            OrderId = order.Id,
            UserId = order.UserId,
            Reason = request.Reason
        }, ct);

        return true;
    }
}