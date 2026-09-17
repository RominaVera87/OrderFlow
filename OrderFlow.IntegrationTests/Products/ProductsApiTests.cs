using System.Net;
using OrderFlow.IntegrationTests.Infrastructure;
using System.Net.Http.Json;
using OrderFlow.Application.DTOs.Products;

namespace OrderFlow.IntegrationTests.Products;

public class ProductsApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task Create_WithValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Gaming Mouse",
            Description = "Wireless gaming mouse",
            Price = 2499m,
            Stock = 10
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/products",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }

    [Fact]
    public async Task Create_WithValidRequest_CanBeRetrievedById()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Mechanical Keyboard",
            Description = "RGB mechanical keyboard",
            Price = 3990m,
            Stock = 7
        };

        // Act
        var createResponse = await _client.PostAsJsonAsync(
            "/api/products",
            request);

        var createdProduct =
            await createResponse.Content
                .ReadFromJsonAsync<ProductResponse>();

        var getResponse = await _client.GetAsync(
            $"/api/products/{createdProduct!.Id}");

        var retrievedProduct =
            await getResponse.Content
                .ReadFromJsonAsync<ProductResponse>();

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode);

        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode);

        Assert.NotNull(retrievedProduct);

        Assert.Equal(
            createdProduct.Id,
            retrievedProduct.Id);

        Assert.Equal(
            "Mechanical Keyboard",
            retrievedProduct.Name);
    }

    [Fact]
    public async Task GetById_WhenProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync(
            $"/api/products/{id}");

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenProductExists_ReturnsNoContent()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Temporary Product",
            Description = "Product created for deletion test",
            Price = 500m,
            Stock = 1
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/products",
            request);

        var createdProduct =
            await createResponse.Content
                .ReadFromJsonAsync<ProductResponse>();

        // Act
        var deleteResponse = await _client.DeleteAsync(
            $"/api/products/{createdProduct!.Id}");

        // Assert
        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);
    }
}