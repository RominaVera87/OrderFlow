namespace OrderFlow.Application.DTOs.Customers;

public class CustomerResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}