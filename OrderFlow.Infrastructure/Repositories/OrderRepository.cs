using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Persistence;

namespace OrderFlow.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderFlowDbContext _dbContext;

    public OrderRepository(OrderFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .OrderByDescending(order => order.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .FirstOrDefaultAsync(
                order => order.Id == id,
                cancellationToken);
    }

    public async Task<Order?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Orders
            .Include(order => order.Items)
            .FirstOrDefaultAsync(
                order => order.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        await _dbContext.Orders.AddAsync(
            order,
            cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}