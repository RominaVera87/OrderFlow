using OrderFlow.Domain.Enums;

namespace OrderFlow.Domain.Entities;

public class Order
{
    private readonly List<OrderItem> _items = [];

    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public OrderStatus Status { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public decimal Total => _items.Sum(item => item.Total);

    private Order()
    {
    }

    public Order(
        Guid id,
        Guid customerId,
        DateTime createdAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "Order id cannot be empty.",
                nameof(id));
        }

        if (customerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Customer id cannot be empty.",
                nameof(customerId));
        }

        Id = id;
        CustomerId = customerId;
        CreatedAtUtc = createdAtUtc;
        Status = OrderStatus.Pending;
    }

    public void AddItem(
        Guid productId,
        int quantity,
        decimal unitPrice)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException(
                "Items can only be added to pending orders.");
        }

        var item = new OrderItem(
            Guid.NewGuid(),
            productId,
            quantity,
            unitPrice);

        _items.Add(item);
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending orders can be confirmed.");
        }

        if (_items.Count == 0)
        {
            throw new InvalidOperationException(
                "An order must contain at least one item.");
        }

        Status = OrderStatus.Confirmed;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
        {
            throw new InvalidOperationException(
                "Only confirmed orders can be shipped.");
        }

        Status = OrderStatus.Shipped;
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
        {
            throw new InvalidOperationException(
                "Only shipped orders can be delivered.");
        }

        Status = OrderStatus.Delivered;
    }

    public void Cancel()
    {
        if (Status != OrderStatus.Pending &&
            Status != OrderStatus.Confirmed)
        {
            throw new InvalidOperationException(
                "Only pending or confirmed orders can be cancelled.");
        }

        Status = OrderStatus.Cancelled;
    }
}