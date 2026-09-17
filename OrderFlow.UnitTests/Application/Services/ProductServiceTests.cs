using Moq;
using OrderFlow.Application.DTOs.Products;
using OrderFlow.Application.Interfaces;
using OrderFlow.Application.Services;
using OrderFlow.Domain.Entities;

namespace OrderFlow.UnitTests.Application.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();

        _service = new ProductService(
            _repositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsProduct()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "Monitor",
            "Full HD",
            1000m,
            5);

        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(
                product.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _service.GetByIdAsync(
            product.Id,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Name, result.Name);
    }


    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(
                id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _service.GetByIdAsync(
            id,
            CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_CreatesProduct()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Mouse Logitech",
            Description = "Mouse inalámbrico",
            Price = 1500m,
            Stock = 10
        };

        Product? capturedProduct = null;

        _repositoryMock
            .Setup(repository => repository.AddAsync(
                It.IsAny<Product>(),
                It.IsAny<CancellationToken>()))
            .Callback<Product, CancellationToken>(
                (product, _) => capturedProduct = product)
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(repository => repository.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(
            request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(capturedProduct);
        Assert.Equal(request.Name, capturedProduct.Name);
        Assert.Equal(request.Price, capturedProduct.Price);

        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Price, result.Price);
    }
}