using Moq;
using OrderFlow.Application.DTOs.Orders;
using OrderFlow.Application.Exceptions;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Services;
using OrderFlow.Domain.Entities;

namespace OrderFlow.UnitTests.Application.Services;

public class OrderServiceTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();

        _service = new OrderService(
            _customerRepositoryMock.Object,
            _productRepositoryMock.Object,
            _orderRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_CreatesOrderAndDecreasesStock()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var customer = new Customer(
            customerId,
            "John Smith",
            "john.smith@example.com");

        var product = new Product(
            productId,
            "Gaming Mouse",
            "Wireless gaming mouse",
            1500m,
            10);

        var request = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items =
            [
                new CreateOrderItemRequest
                {
                    ProductId = productId,
                    Quantity = 2
                }
            ]
        };

        _customerRepositoryMock
            .Setup(repository => repository.GetByIdAsync(
                customerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _productRepositoryMock
            .Setup(repository => repository.GetByIdsForUpdateAsync(
                It.IsAny<IReadOnlyCollection<Guid>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([product]);

        // Act
        var result = await _service.CreateAsync(
            request,
            CancellationToken.None);

        // Assert
        Assert.Equal(customerId, result.CustomerId);
        Assert.Equal(3000m, result.Total);
        Assert.Equal("Pending", result.Status);
        Assert.Single(result.Items);
        Assert.Equal(8, product.Stock);

        _orderRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _orderRepositoryMock.Verify(
            repository => repository.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInsufficientStock_ThrowsInvalidOperationException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var customer = new Customer(
            customerId,
            "John Smith",
            "john.smith@example.com");

        var product = new Product(
            productId,
            "Gaming Mouse",
            "Wireless gaming mouse",
            1500m,
            2);

        var request = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items =
            [
                new CreateOrderItemRequest
                {
                    ProductId = productId,
                    Quantity = 3
                }
            ]
        };

        _customerRepositoryMock
            .Setup(repository => repository.GetByIdAsync(
                customerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _productRepositoryMock
            .Setup(repository => repository.GetByIdsForUpdateAsync(
                It.IsAny<IReadOnlyCollection<Guid>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([product]);

        // Act
        var act = async () => await _service.CreateAsync(
            request,
            CancellationToken.None);

        // Assert
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(act);

        Assert.Equal(
            "Insufficient product stock.",
            exception.Message);

        Assert.Equal(2, product.Stock);

        _orderRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _orderRepositoryMock.Verify(
            repository => repository.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenCustomerDoesNotExist_ThrowsOrderValidationException()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        var request = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items =
            [
                new CreateOrderItemRequest
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 1
                }
            ]
        };

        _customerRepositoryMock
            .Setup(repository => repository.GetByIdAsync(
                customerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var act = async () => await _service.CreateAsync(
            request,
            CancellationToken.None);

        // Assert
        var exception =
            await Assert.ThrowsAsync<OrderValidationException>(act);

        Assert.Equal(
            "Customer was not found.",
            exception.Message);

        _productRepositoryMock.Verify(
            repository => repository.GetByIdsForUpdateAsync(
                It.IsAny<IReadOnlyCollection<Guid>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _orderRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateProducts_ThrowsOrderValidationException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var customer = new Customer(
            customerId,
            "John Smith",
            "john.smith@example.com");

        var request = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items =
            [
                new CreateOrderItemRequest
                {
                    ProductId = productId,
                    Quantity = 1
                },
                new CreateOrderItemRequest
                {
                    ProductId = productId,
                    Quantity = 2
                }
            ]
        };

        _customerRepositoryMock
            .Setup(repository => repository.GetByIdAsync(
                customerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        // Act
        var act = async () => await _service.CreateAsync(
            request,
            CancellationToken.None);

        // Assert
        var exception =
            await Assert.ThrowsAsync<OrderValidationException>(act);

        Assert.Equal(
            "An order cannot contain duplicate products.",
            exception.Message);

        _productRepositoryMock.Verify(
            repository => repository.GetByIdsForUpdateAsync(
                It.IsAny<IReadOnlyCollection<Guid>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _orderRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenProductDoesNotExist_ThrowsOrderValidationException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var customer = new Customer(
            customerId,
            "John Smith",
            "john.smith@example.com");

        var request = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items =
            [
                new CreateOrderItemRequest
                {
                    ProductId = productId,
                    Quantity = 1
                }
            ]
        };

        _customerRepositoryMock
            .Setup(repository => repository.GetByIdAsync(
                customerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _productRepositoryMock
            .Setup(repository => repository.GetByIdsForUpdateAsync(
                It.IsAny<IReadOnlyCollection<Guid>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var act = async () => await _service.CreateAsync(
            request,
            CancellationToken.None);

        // Assert
        var exception =
            await Assert.ThrowsAsync<OrderValidationException>(act);

        Assert.Equal(
            "One or more products were not found.",
            exception.Message);

        _orderRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _orderRepositoryMock.Verify(
            repository => repository.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}