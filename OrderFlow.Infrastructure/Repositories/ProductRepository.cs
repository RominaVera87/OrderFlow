using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Persistence;

namespace OrderFlow.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly OrderFlowDbContext _dbContext;

    public ProductRepository(OrderFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(
                product => product.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetInStockAsync(
        CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Where(product => product.Stock > 0)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Product product,
        CancellationToken cancellationToken)
    {
        await _dbContext.Products.AddAsync(
            product,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdForUpdateAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .FirstOrDefaultAsync(
                product => product.Id == id,
                cancellationToken);
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(
                product => product.Id == id,
                cancellationToken);

        if (product is null)
        {
            return false;
        }

        _dbContext.Products.Remove(product);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IReadOnlyList<Product>> GetByIdsForUpdateAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .Where(product => ids.Contains(product.Id))
            .ToListAsync(cancellationToken);
    }
}