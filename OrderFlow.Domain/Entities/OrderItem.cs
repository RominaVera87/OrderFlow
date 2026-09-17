namespace OrderFlow.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }

    public Guid ProductId { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal Total => UnitPrice * Quantity;

    private OrderItem()
    {
    }

    public OrderItem(
        Guid id,
        Guid productId,
        int quantity,
        decimal unitPrice)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "Order item id cannot be empty.",
                nameof(id));
        }

        if (productId == Guid.Empty)
        {
            throw new ArgumentException(
                "Product id cannot be empty.",
                nameof(productId));
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantity must be greater than zero.");
        }

        if (unitPrice <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(unitPrice),
                "Unit price must be greater than zero.");
        }

        Id = id;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}