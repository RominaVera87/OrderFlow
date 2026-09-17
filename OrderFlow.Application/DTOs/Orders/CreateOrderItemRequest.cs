using System.ComponentModel.DataAnnotations;

namespace OrderFlow.Application.DTOs.Orders;

public class CreateOrderItemRequest
{
    public Guid ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}