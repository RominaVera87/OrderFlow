using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Application.DTOs.Auth;
using OrderFlow.Application.DTOs.Customers;
using OrderFlow.Application.DTOs.Orders;
using OrderFlow.Application.DTOs.Products;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;
using OrderFlow.Infrastructure.Persistence;
using OrderFlow.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using OrderFlow.Application.Interfaces;

namespace OrderFlow.IntegrationTests.Orders;

public class OrdersApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public OrdersApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "no-such-user@example.com",
            Password = "invalid-password"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            loginRequest);

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task Create_WithValidRequest_CreatesOrderAndDecreasesStock()
    {
        // Arrange
        await AuthenticateAsync();
        var customer = await CreateCustomerAsync();
        var product = await CreateProductAsync(10);

        // Act
        var order = await CreateOrderAsync(
            customer.Id,
            product.Id,
            2);

        // Assert
        Assert.Equal("Pending", order.Status);
        Assert.Equal(3000m, order.Total);
        Assert.Single(order.Items);

        var updatedProduct =
            await GetProductAsync(product.Id);

        Assert.Equal(8, updatedProduct.Stock);

        await AssertOrderWasPersistedAsync(order.Id);
    }

    [Fact]
    public async Task GetById_WhenOrderExists_ReturnsOrder()
    {
        // Arrange
        await AuthenticateAsync();

        var customer = await CreateCustomerAsync();
        var product = await CreateProductAsync(10);

        var createdOrder = await CreateOrderAsync(
            customer.Id,
            product.Id,
            1);

        // Act
        var response = await _client.GetAsync(
            $"/api/orders/{createdOrder.Id}");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var order =
            await response.Content
                .ReadFromJsonAsync<OrderResponse>();

        Assert.NotNull(order);
        Assert.Equal(createdOrder.Id, order.Id);
        Assert.Equal("Pending", order.Status);
    }

    [Fact]
    public async Task GetAll_ReturnsCreatedOrder()
    {
        // Arrange
        await AuthenticateAsync();

        var customer = await CreateCustomerAsync();
        var product = await CreateProductAsync(10);

        var createdOrder = await CreateOrderAsync(
            customer.Id,
            product.Id,
            1);

        // Act
        var response = await _client.GetAsync(
            "/api/orders");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var orders =
            await response.Content
                .ReadFromJsonAsync<List<OrderResponse>>();

        Assert.NotNull(orders);

        Assert.Contains(
            orders,
            order => order.Id == createdOrder.Id);
    }

    [Fact]
    public async Task Confirm_WhenOrderIsPending_ChangesStatusToConfirmed()
    {
        // Arrange
        await AuthenticateAsync();
        var order = await CreateTestOrderAsync();

        // Act
        var response = await _client.PostAsync(
            $"/api/orders/{order.Id}/confirm",
            null);

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var updatedOrder =
            await response.Content
                .ReadFromJsonAsync<OrderResponse>();

        Assert.NotNull(updatedOrder);
        Assert.Equal("Confirmed", updatedOrder.Status);
    }

    [Fact]
    public async Task Ship_WhenUserIsCustomer_ReturnsForbidden()
    {
        // Arrange
        await AuthenticateAsync();

        var order = await CreateTestOrderAsync();

        await _client.PostAsync(
            $"/api/orders/{order.Id}/confirm",
            null);

        // Act
        var response = await _client.PostAsync(
            $"/api/orders/{order.Id}/ship",
            null);

        // Assert
        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    [Fact]
    public async Task Ship_WhenUserIsAdmin_ChangesStatusToShipped()
    {
        // Arrange
        await AuthenticateAsync();

        var order = await CreateTestOrderAsync();

        await _client.PostAsync(
            $"/api/orders/{order.Id}/confirm",
            null);

        await AuthenticateAsAdminAsync();

        // Act
        var response = await _client.PostAsync(
            $"/api/orders/{order.Id}/ship",
            null);

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var updatedOrder =
            await response.Content
                .ReadFromJsonAsync<OrderResponse>();

        Assert.NotNull(updatedOrder);
        Assert.Equal("Shipped", updatedOrder.Status);
    }

    [Fact]
    public async Task Deliver_WhenOrderIsShipped_ChangesStatusToDelivered()
    {
        // Arrange
        await AuthenticateAsync();

        var order = await CreateTestOrderAsync();

        await _client.PostAsync(
            $"/api/orders/{order.Id}/confirm",
            null);

        await AuthenticateAsAdminAsync();

        await _client.PostAsync(
            $"/api/orders/{order.Id}/ship",
            null);

        // Act
        var response = await _client.PostAsync(
            $"/api/orders/{order.Id}/deliver",
            null);

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var updatedOrder =
            await response.Content
                .ReadFromJsonAsync<OrderResponse>();

        Assert.NotNull(updatedOrder);
        Assert.Equal("Delivered", updatedOrder.Status);
    }

    [Fact]
    public async Task Cancel_WhenOrderIsPending_RestoresProductStock()
    {
        // Arrange
        await AuthenticateAsync();
        var customer = await CreateCustomerAsync();
        var product = await CreateProductAsync(10);

        var order = await CreateOrderAsync(
            customer.Id,
            product.Id,
            2);

        var productAfterOrder =
            await GetProductAsync(product.Id);

        Assert.Equal(8, productAfterOrder.Stock);

        // Act
        var response = await _client.PostAsync(
            $"/api/orders/{order.Id}/cancel",
            null);

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var cancelledOrder =
            await response.Content
                .ReadFromJsonAsync<OrderResponse>();

        Assert.NotNull(cancelledOrder);
        Assert.Equal("Cancelled", cancelledOrder.Status);

        var productAfterCancellation =
            await GetProductAsync(product.Id);

        Assert.Equal(10, productAfterCancellation.Stock);
    }

    [Fact]
    public async Task Ship_WhenOrderIsPending_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsync();

        var order = await CreateTestOrderAsync();

        await AuthenticateAsAdminAsync();

        // Act
        var response = await _client.PostAsync(
            $"/api/orders/{order.Id}/ship",
            null);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task Deliver_WhenOrderIsPending_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsync();
        var order = await CreateTestOrderAsync();

        // Act
        var response = await _client.PostAsync(
            $"/api/orders/{order.Id}/deliver",
            null);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task Cancel_WhenOrderIsDelivered_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsync();

        var order = await CreateTestOrderAsync();

        await _client.PostAsync(
            $"/api/orders/{order.Id}/confirm",
            null);

        await AuthenticateAsAdminAsync();

        await _client.PostAsync(
            $"/api/orders/{order.Id}/ship",
            null);

        await _client.PostAsync(
            $"/api/orders/{order.Id}/deliver",
            null);

        // Act
        var response = await _client.PostAsync(
            $"/api/orders/{order.Id}/cancel",
            null);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task Confirm_WhenOrderIsCancelled_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsync();
        var order = await CreateTestOrderAsync();

        await _client.PostAsync(
            $"/api/orders/{order.Id}/cancel",
            null);

        // Act
        var response = await _client.PostAsync(
            $"/api/orders/{order.Id}/confirm",
            null);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync(
            "/api/orders");

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    private async Task<OrderResponse> CreateTestOrderAsync()
    {
        var customer = await CreateCustomerAsync();
        var product = await CreateProductAsync(10);

        return await CreateOrderAsync(
            customer.Id,
            product.Id,
            1);
    }

    private async Task<CustomerResponse> CreateCustomerAsync()
    {
        var request = new CreateCustomerRequest
        {
            Name = $"Integration Customer {Guid.NewGuid()}",
            Email = $"customer-{Guid.NewGuid()}@example.com"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/customers",
            request);

        response.EnsureSuccessStatusCode();

        var customer =
            await response.Content
                .ReadFromJsonAsync<CustomerResponse>();

        Assert.NotNull(customer);

        return customer;
    }

    private async Task<ProductResponse> CreateProductAsync(
        int stock)
    {
        var request = new CreateProductRequest
        {
            Name = $"Integration Product {Guid.NewGuid()}",
            Description = "Product created by an integration test",
            Price = 1500m,
            Stock = stock
        };

        var response = await _client.PostAsJsonAsync(
            "/api/products",
            request);

        response.EnsureSuccessStatusCode();

        var product =
            await response.Content
                .ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(product);

        return product;
    }

    private async Task<OrderResponse> CreateOrderAsync(
        Guid customerId,
        Guid productId,
        int quantity)
    {
        var request = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items =
            [
                new CreateOrderItemRequest
                {
                    ProductId = productId,
                    Quantity = quantity
                }
            ]
        };

        var response = await _client.PostAsJsonAsync(
            "/api/orders",
            request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var order =
            await response.Content
                .ReadFromJsonAsync<OrderResponse>();

        Assert.NotNull(order);

        return order;
    }

    private async Task<ProductResponse> GetProductAsync(
        Guid productId)
    {
        var product =
            await _client.GetFromJsonAsync<ProductResponse>(
                $"/api/products/{productId}");

        Assert.NotNull(product);

        return product;
    }

    private async Task AssertOrderWasPersistedAsync(
        Guid orderId)
    {
        using var scope =
            _factory.Services.CreateScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<OrderFlowDbContext>();

        var order =
            await dbContext.Orders.FindAsync(orderId);

        Assert.NotNull(order);
    }


    private async Task AuthenticateAsync()
    {
        var request = new RegisterRequest
        {
            Email = $"order-test-{Guid.NewGuid()}@example.com",
            Password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            request);

        response.EnsureSuccessStatusCode();

        var authResponse =
            await response.Content
                .ReadFromJsonAsync<AuthResponse>();

        Assert.NotNull(authResponse);
        Assert.False(string.IsNullOrWhiteSpace(authResponse.Token));

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                authResponse.Token);
    }

    private async Task AuthenticateAsAdminAsync()
    {
        var email = $"admin-order-tests-{Guid.NewGuid()}@example.com";
        const string password = "Password123!";

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext =
                scope.ServiceProvider
                    .GetRequiredService<OrderFlowDbContext>();

            var passwordHasher =
                scope.ServiceProvider
                    .GetRequiredService<IPasswordHasher>();

            var passwordHash =
                passwordHasher.Hash(password);

            var user = new User(
                Guid.NewGuid(),
                email,
                passwordHash,
                UserRole.Admin,
                DateTime.UtcNow);

            dbContext.Users.Add(user);

            await dbContext.SaveChangesAsync();
        }

        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            loginRequest);

        response.EnsureSuccessStatusCode();

        var authResponse =
            await response.Content
                .ReadFromJsonAsync<AuthResponse>();

        Assert.NotNull(authResponse);
        Assert.False(
            string.IsNullOrWhiteSpace(authResponse.Token));

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                authResponse.Token);
    }
}