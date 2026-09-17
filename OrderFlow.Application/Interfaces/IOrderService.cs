using OrderFlow.Application.DTOs.Orders;

namespace OrderFlow.Application.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyList<OrderResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken);

    Task<OrderResponse?> ConfirmAsync(Guid id, CancellationToken cancellationToken);

    Task<OrderResponse?> ShipAsync(Guid id, CancellationToken cancellationToken);

    Task<OrderResponse?> DeliverAsync(Guid id, CancellationToken cancellationToken);

    Task<OrderResponse?> CancelAsync(Guid id, CancellationToken cancellationToken);
}