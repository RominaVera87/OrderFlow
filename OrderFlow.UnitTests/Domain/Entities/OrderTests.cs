using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;

namespace OrderFlow.UnitTests.Domain.Entities;

public class OrderTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesPendingOrder()
    {
        // Arrange
        var id = Guid.NewGuid();
        var customerId = Guid.NewGuid();

        // Act
        var order = new Order(
            id,
            customerId,
            DateTime.UtcNow);

        // Assert
        Assert.Equal(id, order.Id);
        Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Empty(order.Items);
        Assert.Equal(0m, order.Total);
    }

    [Fact]
    public void AddItem_WithValidData_AddsItemAndCalculatesTotal()
    {
        // Arrange
        var order = CreateOrder();
        var productId = Guid.NewGuid();

        // Act
        order.AddItem(
            productId,
            2,
            1500m);

        // Assert
        Assert.Single(order.Items);
        Assert.Equal(3000m, order.Total);

        var item = order.Items.Single();

        Assert.Equal(productId, item.ProductId);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(1500m, item.UnitPrice);
        Assert.Equal(3000m, item.Total);
    }

    [Fact]
    public void AddItem_WithMultipleItems_CalculatesOrderTotal()
    {
        // Arrange
        var order = CreateOrder();

        // Act
        order.AddItem(
            Guid.NewGuid(),
            2,
            1000m);

        order.AddItem(
            Guid.NewGuid(),
            1,
            500m);

        // Assert
        Assert.Equal(2500m, order.Total);
        Assert.Equal(2, order.Items.Count);
    }

    [Fact]
    public void Confirm_WhenOrderIsPending_ChangesStatusToConfirmed()
    {
        // Arrange
        var order = CreateOrderWithItem();

        // Act
        order.Confirm();

        // Assert
        Assert.Equal(
            OrderStatus.Confirmed,
            order.Status);
    }

    [Fact]
    public void Confirm_WhenOrderHasNoItems_ThrowsInvalidOperationException()
    {
        // Arrange
        var order = CreateOrder();

        // Act
        var act = () => order.Confirm();

        // Assert
        var exception =
            Assert.Throws<InvalidOperationException>(act);

        Assert.Equal(
            "An order must contain at least one item.",
            exception.Message);
    }

    [Fact]
    public void Ship_WhenOrderIsConfirmed_ChangesStatusToShipped()
    {
        // Arrange
        var order = CreateOrderWithItem();

        order.Confirm();

        // Act
        order.Ship();

        // Assert
        Assert.Equal(
            OrderStatus.Shipped,
            order.Status);
    }

    [Fact]
    public void Ship_WhenOrderIsPending_ThrowsInvalidOperationException()
    {
        // Arrange
        var order = CreateOrderWithItem();

        // Act
        var act = () => order.Ship();

        // Assert
        var exception =
            Assert.Throws<InvalidOperationException>(act);

        Assert.Equal(
            "Only confirmed orders can be shipped.",
            exception.Message);
    }

    [Fact]
    public void Deliver_WhenOrderIsShipped_ChangesStatusToDelivered()
    {
        // Arrange
        var order = CreateOrderWithItem();

        order.Confirm();
        order.Ship();

        // Act
        order.Deliver();

        // Assert
        Assert.Equal(
            OrderStatus.Delivered,
            order.Status);
    }

    [Fact]
    public void Deliver_WhenOrderIsPending_ThrowsInvalidOperationException()
    {
        // Arrange
        var order = CreateOrderWithItem();

        // Act
        var act = () => order.Deliver();

        // Assert
        var exception =
            Assert.Throws<InvalidOperationException>(act);

        Assert.Equal(
            "Only shipped orders can be delivered.",
            exception.Message);
    }

    [Fact]
    public void Cancel_WhenOrderIsPending_ChangesStatusToCancelled()
    {
        // Arrange
        var order = CreateOrderWithItem();

        // Act
        order.Cancel();

        // Assert
        Assert.Equal(
            OrderStatus.Cancelled,
            order.Status);
    }

    [Fact]
    public void Cancel_WhenOrderIsConfirmed_ChangesStatusToCancelled()
    {
        // Arrange
        var order = CreateOrderWithItem();

        order.Confirm();

        // Act
        order.Cancel();

        // Assert
        Assert.Equal(
            OrderStatus.Cancelled,
            order.Status);
    }

    [Fact]
    public void Cancel_WhenOrderIsDelivered_ThrowsInvalidOperationException()
    {
        // Arrange
        var order = CreateOrderWithItem();

        order.Confirm();
        order.Ship();
        order.Deliver();

        // Act
        var act = () => order.Cancel();

        // Assert
        var exception =
            Assert.Throws<InvalidOperationException>(act);

        Assert.Equal(
            "Only pending or confirmed orders can be cancelled.",
            exception.Message);
    }

    [Fact]
    public void Confirm_WhenOrderIsCancelled_ThrowsInvalidOperationException()
    {
        // Arrange
        var order = CreateOrderWithItem();

        order.Cancel();

        // Act
        var act = () => order.Confirm();

        // Assert
        var exception =
            Assert.Throws<InvalidOperationException>(act);

        Assert.Equal(
            "Only pending orders can be confirmed.",
            exception.Message);
    }

    [Fact]
    public void Confirm_WhenOrderIsAlreadyConfirmed_ThrowsInvalidOperationException()
    {
        // Arrange
        var order = CreateOrderWithItem();

        order.Confirm();

        // Act
        var act = () => order.Confirm();

        // Assert
        var exception =
            Assert.Throws<InvalidOperationException>(act);

        Assert.Equal(
            "Only pending orders can be confirmed.",
            exception.Message);
    }

    [Fact]
    public void AddItem_WhenOrderIsConfirmed_ThrowsInvalidOperationException()
    {
        // Arrange
        var order = CreateOrderWithItem();

        order.Confirm();

        // Act
        var act = () => order.AddItem(
            Guid.NewGuid(),
            1,
            1000m);

        // Assert
        var exception =
            Assert.Throws<InvalidOperationException>(act);

        Assert.Equal(
            "Items can only be added to pending orders.",
            exception.Message);
    }

    private static Order CreateOrder()
    {
        return new Order(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow);
    }

    private static Order CreateOrderWithItem()
    {
        var order = CreateOrder();

        order.AddItem(
            Guid.NewGuid(),
            1,
            1000m);

        return order;
    }
}