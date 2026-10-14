using AwesomeAssertions;
using FStore.EventBus;
using FStore.EventBus.Events;
using FStore.Order.Application.Features.Orders.Commands.CancelOrder;
using FStore.Order.Domain.Entities;
using FStore.Order.Domain.Enums;
using FStore.Order.UnitTests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace FStore.Order.UnitTests.Features.Orders;

public class CancelOrderCommandHandlerTests
{
    [Fact]
    public async Task HandleWithPendingOrderCancelsAndPublishesEvent()
    {
        using var db = InMemoryDbContextFactory.Create();
        var order = new OrderEntity
        {
            UserId = Guid.NewGuid(),
            ShippingAddress = "123 Main St"
        };
        order.AddItem(Guid.NewGuid(), "Laptop", 100m, 1);
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        var eventBus = Substitute.For<IEventBus>();
        var handler = new CancelOrderCommandHandler(db, eventBus);

        var result = await handler.Handle(
            new CancelOrderCommand(order.Id, "Customer changed mind"),
            CancellationToken.None);

        result.Should().BeTrue();

        var updated = await db.Orders.FindAsync(order.Id);
        updated!.Status.Should().Be(OrderStatus.Cancelled);
        updated.CancellationReason.Should().Be("Customer changed mind");

        await eventBus.Received(1).PublishAsync(
            Arg.Is<OrderCancelledEvent>(e =>
                e.OrderId == order.Id &&
                e.Reason == "Customer changed mind"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleWithMissingOrderReturnsFalse()
    {
        using var db = InMemoryDbContextFactory.Create();
        var eventBus = Substitute.For<IEventBus>();
        var handler = new CancelOrderCommandHandler(db, eventBus);

        var result = await handler.Handle(
            new CancelOrderCommand(Guid.NewGuid(), "reason"),
            CancellationToken.None);

        result.Should().BeFalse();
        await eventBus.DidNotReceiveWithAnyArgs()
            .PublishAsync(Arg.Any<OrderCancelledEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleWithShippedOrderThrowsInvalidOperationException()
    {
        using var db = InMemoryDbContextFactory.Create();
        var order = new OrderEntity { UserId = Guid.NewGuid(), ShippingAddress = "X" };
        order.AddItem(Guid.NewGuid(), "Item", 10m, 1);
        order.MarkAsConfirmed();
        order.MarkAsShipped();
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        var handler = new CancelOrderCommandHandler(db, Substitute.For<IEventBus>());

        var act = () => handler.Handle(
            new CancelOrderCommand(order.Id, "reason"),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Shipped*");
    }
}