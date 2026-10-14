using AwesomeAssertions;
using FStore.Order.Domain.Entities;
using FStore.Order.Domain.Enums;

namespace FStore.Order.UnitTests.Domain;

public class OrderTests
{
    [Fact]
    public void NewOrderStartsInPendingStatus()
    {
        var order = new OrderEntity { UserId = Guid.NewGuid(), ShippingAddress = "X" };

        order.Status.Should().Be(OrderStatus.Pending);
        order.Items.Should().BeEmpty();
        order.TotalAmount.Should().Be(0);
    }

    [Fact]
    public void AddItemWithValidDataAddsToCollection()
    {
        var order = new OrderEntity { UserId = Guid.NewGuid(), ShippingAddress = "X" };

        order.AddItem(Guid.NewGuid(), "Laptop", 1499.99m, 2);

        order.Items.Should().HaveCount(1);
        order.Items.First().LineTotal.Should().Be(1499.99m * 2);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddItemWithInvalidQuantityThrows(int quantity)
    {
        var order = new OrderEntity { UserId = Guid.NewGuid(), ShippingAddress = "X" };

        var act = () => order.AddItem(Guid.NewGuid(), "X", 10m, quantity);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MarkAsConfirmedFromPendingSucceeds()
    {
        var order = new OrderEntity { UserId = Guid.NewGuid(), ShippingAddress = "X" };

        order.MarkAsConfirmed();

        order.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public void MarkAsConfirmedFromCancelledThrows()
    {
        var order = new OrderEntity { UserId = Guid.NewGuid(), ShippingAddress = "X" };
        order.MarkAsCancelled("reason");

        var act = () => order.MarkAsConfirmed();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cancelled*");
    }

    [Fact]
    public void MarkAsCancelledFromShippedThrows()
    {
        var order = new OrderEntity { UserId = Guid.NewGuid(), ShippingAddress = "X" };
        order.MarkAsConfirmed();
        order.MarkAsShipped();

        var act = () => order.MarkAsCancelled("reason");

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Shipped*");
    }

    [Fact]
    public void FullLifecycleHappyPathTransitionsCorrectly()
    {
        var order = new OrderEntity { UserId = Guid.NewGuid(), ShippingAddress = "X" };

        order.Status.Should().Be(OrderStatus.Pending);
        order.MarkAsConfirmed();
        order.Status.Should().Be(OrderStatus.Confirmed);
        order.MarkAsShipped();
        order.Status.Should().Be(OrderStatus.Shipped);
        order.MarkAsDelivered();
        order.Status.Should().Be(OrderStatus.Delivered);
    }
}