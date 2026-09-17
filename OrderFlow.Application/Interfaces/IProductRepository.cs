using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Interfaces;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken);

    Task<Product?> GetByIdAsync(Guid id,CancellationToken cancellationToken);

    Task<IReadOnlyList<Product>> GetInStockAsync(CancellationToken cancellationToken);

    Task AddAsync(Product product, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<Product?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Product>> GetByIdsForUpdateAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken);

}