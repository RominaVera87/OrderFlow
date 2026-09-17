using OrderFlow.Application.DTOs.Products;

namespace OrderFlow.Application.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<ProductResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<ProductResponse>> GetInStockAsync(CancellationToken cancellationToken);

    Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);

    Task<ProductResponse?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}