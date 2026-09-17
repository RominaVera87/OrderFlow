using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Persistence;

namespace OrderFlow.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly OrderFlowDbContext _dbContext;

    public UserRepository(OrderFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail =
            email.Trim().ToLowerInvariant();

        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                user => user.Email == normalizedEmail,
                cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddAsync(
            user,
            cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}