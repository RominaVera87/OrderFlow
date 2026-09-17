using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Persistence;

namespace OrderFlow.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly OrderFlowDbContext _dbContext;

    public CustomerRepository(OrderFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                customer => customer.Id == id,
                cancellationToken);
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return await _dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                customer => customer.Email == normalizedEmail,
                cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        await _dbContext.Customers.AddAsync(
            customer,
            cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}