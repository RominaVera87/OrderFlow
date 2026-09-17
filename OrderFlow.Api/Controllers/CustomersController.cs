using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.DTOs.Customers;
using OrderFlow.Application.Interfaces;

namespace OrderFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var customers = await _customerService.GetAllAsync(cancellationToken);

        return Ok(customers);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetByIdAsync(id, cancellationToken);

        if (customer is null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> Create(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = await _customerService.CreateAsync(request, cancellationToken);

        if (customer is null)
        {
            return Conflict(new
            {
                Message = "A customer with this email already exists."
            });
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = customer.Id },
            customer);
    }
}