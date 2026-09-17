using System.ComponentModel.DataAnnotations;

namespace OrderFlow.Application.DTOs.Customers;

public class CreateCustomerRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(250)]
    public string Email { get; set; } = string.Empty;
}