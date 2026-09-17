using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderFlow.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace OrderFlow.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>,
      IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer =
        new PostgreSqlBuilder()
            .WithDatabase("orderflow_tests")
            .WithUsername("postgres")
            .WithPassword("admin")
            .Build();

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        using var scope = Services.CreateScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<OrderFlowDbContext>();

        await dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<OrderFlowDbContext>>();

            services.RemoveAll<IDbContextOptionsConfiguration<OrderFlowDbContext>>();

            services.AddDbContext<OrderFlowDbContext>(options =>
            {
                options.UseNpgsql(_postgresContainer.GetConnectionString());
            });
        });
    }
}   