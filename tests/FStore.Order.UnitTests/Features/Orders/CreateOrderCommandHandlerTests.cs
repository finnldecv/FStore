using AwesomeAssertions;
using FStore.EventBus;
using FStore.EventBus.Events;
using FStore.Order.Application.Features.Orders.Commands.CreateOrder;
using FStore.Order.Domain.Enums;
using FStore.Order.UnitTests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace FStore.Order.UnitTests.Features.Orders;

public class CreateOrderCommandHandlerTests
{
    [Fact]
    public async Task HandleWithValidCommandCreatesOrderAndReturnsDto()
    {
        using var db = InMemoryDbContextFactory.Create();
        var eventBus = Substitute.For<IEventBus>();
        var handler = new CreateOrderCommandHandler(db, eventBus);

        var userId = Guid.NewGuid();
        var command = new CreateOrderCommand(
            userId,
            "123 Main St",
            "Leave at door",
            new List<CreateOrderItemDto>
            {
                new(Guid.NewGuid(), "Laptop", 1499.99m, 1),
                new(Guid.NewGuid(), "Mouse", 29.99m, 2)
            });

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.Status.Should().Be(nameof(OrderStatus.Pending));
        result.Items.Should().HaveCount(2);
        result.TotalAmount.Should().Be(1499.99m + 29.99m * 2);

        var saved = await db.Orders.Include(o => o.Items).FirstAsync();
        saved.UserId.Should().Be(userId);
        saved.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task HandleWithValidCommandPublishesOrderCreatedEvent()
    {
        using var db = InMemoryDbContextFactory.Create();
        var eventBus = Substitute.For<IEventBus>();
        var handler = new CreateOrderCommandHandler(db, eventBus);
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var command = new CreateOrderCommand(
            userId,
            "123 Main St",
            null,
            new List<CreateOrderItemDto>
            {
                new(productId, "Laptop", 1499.99m, 1)
            });

        await handler.Handle(command, CancellationToken.None);

        await eventBus.Received(1).PublishAsync(
            Arg.Is<OrderCreatedEvent>(e =>
                e.UserId == userId &&
                e.Items.Count == 1 &&
                e.Items[0].ProductId == productId &&
                e.Items[0].Quantity == 1),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleWithMultipleItemsCalculatesTotalCorrectly()
    {
        using var db = InMemoryDbContextFactory.Create();
        var eventBus = Substitute.For<IEventBus>();
        var handler = new CreateOrderCommandHandler(db, eventBus);

        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            "Addr",
            null,
            new List<CreateOrderItemDto>
            {
                new(Guid.NewGuid(), "A", 10m, 3),
                new(Guid.NewGuid(), "B", 5.50m, 4),
                new(Guid.NewGuid(), "C", 100m, 1)
            });

        var result = await handler.Handle(command, CancellationToken.None);

        result.TotalAmount.Should().Be(152m);
    }
}