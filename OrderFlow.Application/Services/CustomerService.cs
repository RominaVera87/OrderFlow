using OrderFlow.Application.DTOs.Customers;
using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IReadOnlyList<CustomerResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetAllAsync(cancellationToken);

        return customers
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<CustomerResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);

        return customer is null ? null : MapToResponse(customer);
    }

    public async Task<CustomerResponse?> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var existingCustomer =
            await _customerRepository.GetByEmailAsync(
                request.Email,
                cancellationToken);

        if (existingCustomer is not null)
        {
            return null;
        }

        var customer = new Customer(
            Guid.NewGuid(),
            request.Name,
            request.Email);

        await _customerRepository.AddAsync(customer,cancellationToken);

        await _customerRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(customer);
    }

    private static CustomerResponse MapToResponse(Customer customer)
    {
        return new CustomerResponse
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email
        };
    }
}