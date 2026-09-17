using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Interfaces;

public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken);

    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Order?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(Order order, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}