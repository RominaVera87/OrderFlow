using System.ComponentModel.DataAnnotations;

namespace OrderFlow.Application.DTOs.Orders;

public class CreateOrderRequest
{
    public Guid CustomerId { get; set; }

    [Required]
    [MinLength(1)]
    public List<CreateOrderItemRequest> Items { get; set; } = [];
}