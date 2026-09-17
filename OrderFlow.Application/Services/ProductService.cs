using OrderFlow.Application.DTOs.Products;
using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(
            cancellationToken);

        return products
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<ProductResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(
            id,
            cancellationToken);

        return product is null
            ? null
            : MapToResponse(product);
    }

    public async Task<IReadOnlyList<ProductResponse>> GetInStockAsync(
        CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetInStockAsync(
            cancellationToken);

        return products
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = new Product(
            Guid.NewGuid(),
            request.Name,
            request.Description,
            request.Price,
            request.Stock);

        await _productRepository.AddAsync(
            product,
            cancellationToken);

        await _productRepository.SaveChangesAsync(
            cancellationToken);

        return MapToResponse(product);
    }

    private static ProductResponse MapToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock
        };
    }

    public async Task<ProductResponse?> UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdForUpdateAsync(
            id,
            cancellationToken);

        if (product is null)
        {
            return null;
        }

        product.Update(
            request.Name,
            request.Description,
            request.Price,
            request.Stock);

        await _productRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(product);
    }

    public async Task<bool> DeleteAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        return await _productRepository.DeleteAsync(
            id,
            cancellationToken);
    }
}