namespace OrderFlow.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public int Stock { get; private set; }

    public uint Version { get; private set; }

    private Product()
    {
        Name = string.Empty;
    }

    public Product(
        Guid id,
        string name,
        string? description,
        decimal price,
        int stock)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "Product id cannot be empty.",
                nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Product name is required.",
                nameof(name));
        }

        if (price <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(price),
                "Product price must be greater than zero.");
        }

        if (stock < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(stock),
                "Product stock cannot be negative.");
        }

        Id = id;
        Name = name.Trim();
        Description = description?.Trim();
        Price = price;
        Stock = stock;
    }

    public void Update(
        string name,
        string? description,
        decimal price,
        int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Product name is required.",
                nameof(name));
        }

        if (price <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(price),
                "Product price must be greater than zero.");
        }

        if (stock < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(stock),
                "Product stock cannot be negative.");
        }

        Name = name.Trim();
        Description = description?.Trim();
        Price = price;
        Stock = stock;
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantity must be greater than zero.");
        }

        if (quantity > Stock)
        {
            throw new InvalidOperationException(
                "Insufficient product stock.");
        }

        Stock -= quantity;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "Quantity must be greater than zero.");
        }

        Stock += quantity;
    }
}
