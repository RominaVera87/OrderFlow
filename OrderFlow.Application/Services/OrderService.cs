using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.DTOs.Orders;
using OrderFlow.Application.Exceptions;
using OrderFlow.Application.Interfaces;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Services;

public class OrderService : IOrderService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderService(
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        IOrderRepository orderRepository)
    {
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<OrderResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);

        return orders
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(
            id,
            cancellationToken);

        return order is null
            ? null
            : MapToResponse(order);
    }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(
            request.CustomerId,
            cancellationToken);

        if (customer is null)
        {
            throw new OrderValidationException(
                "Customer was not found.");
        }

        var hasDuplicateProducts = request.Items
            .GroupBy(item => item.ProductId)
            .Any(group => group.Count() > 1);

        if (hasDuplicateProducts)
        {
            throw new OrderValidationException(
                "An order cannot contain duplicate products.");
        }

        var productIds = request.Items
            .Select(item => item.ProductId)
            .Distinct()
            .ToList();

        var products =
            await _productRepository.GetByIdsForUpdateAsync(
                productIds,
                cancellationToken);

        if (products.Count != productIds.Count)
        {
            throw new OrderValidationException(
                "One or more products were not found.");
        }

        var order = new Order(
            Guid.NewGuid(),
            request.CustomerId,
            DateTime.UtcNow);

        foreach (var requestItem in request.Items)
        {
            var product = products.Single(
                product => product.Id == requestItem.ProductId);

            product.DecreaseStock(requestItem.Quantity);

            order.AddItem(
                product.Id,
                requestItem.Quantity,
                product.Price);
        }

        await _orderRepository.AddAsync(
            order,
            cancellationToken);

        await SaveChangesAsync(cancellationToken);

        return MapToResponse(order);
    }

    public async Task<OrderResponse?> ConfirmAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdForUpdateAsync(
            id,
            cancellationToken);

        if (order is null)
        {
            return null;
        }

        order.Confirm();

        await SaveChangesAsync(cancellationToken);

        return MapToResponse(order);
    }

    public async Task<OrderResponse?> ShipAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdForUpdateAsync(
            id,
            cancellationToken);

        if (order is null)
        {
            return null;
        }

        order.Ship();

        await SaveChangesAsync(cancellationToken);

        return MapToResponse(order);
    }

    public async Task<OrderResponse?> DeliverAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdForUpdateAsync(
            id,
            cancellationToken);

        if (order is null)
        {
            return null;
        }

        order.Deliver();

        await SaveChangesAsync(cancellationToken);

        return MapToResponse(order);
    }

    public async Task<OrderResponse?> CancelAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdForUpdateAsync(
            id,
            cancellationToken);

        if (order is null)
        {
            return null;
        }

        var productIds = order.Items
            .Select(item => item.ProductId)
            .ToList();

        var products =
            await _productRepository.GetByIdsForUpdateAsync(
                productIds,
                cancellationToken);

        if (products.Count != productIds.Count)
        {
            throw new OrderValidationException(
                "One or more order products were not found.");
        }

        order.Cancel();

        foreach (var item in order.Items)
        {
            var product = products.Single(
                product => product.Id == item.ProductId);

            product.IncreaseStock(item.Quantity);
        }

        await SaveChangesAsync(cancellationToken);

        return MapToResponse(order);
    }

    private async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _orderRepository.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new OrderValidationException(
                "The data changed while the operation was being processed. Please try again.");
        }
    }

    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CreatedAtUtc = order.CreatedAtUtc,
            Status = order.Status.ToString(),
            Total = order.Total,

            Items = order.Items
                .Select(item => new OrderItemResponse
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Total = item.Total
                })
                .ToList()
        };
    }
}