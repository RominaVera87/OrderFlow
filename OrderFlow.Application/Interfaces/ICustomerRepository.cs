using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Interfaces;

public interface ICustomerRepository
{
    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken);

    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    Task AddAsync(Customer customer, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}