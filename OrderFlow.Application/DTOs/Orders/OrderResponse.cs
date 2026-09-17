namespace OrderFlow.Application.DTOs.Orders;

public class OrderResponse
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal Total { get; set; }

    public IReadOnlyList<OrderItemResponse> Items { get; set; } = [];
}