using OrderFlow.Application.DTOs.Customers;

namespace OrderFlow.Application.Interfaces;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<CustomerResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<CustomerResponse?> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken);
}