using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.DTOs.Orders;
using OrderFlow.Application.Interfaces;

namespace OrderFlow.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetAllAsync(cancellationToken);

        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderService.GetByIdAsync(
            id,
            cancellationToken);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var order = await _orderService.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = order.Id },
            order);
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<ActionResult<OrderResponse>> Confirm(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderService.ConfirmAsync(
            id,
            cancellationToken);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpPost("{id:guid}/ship")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<OrderResponse>> Ship(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderService.ShipAsync(
            id,
            cancellationToken);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpPost("{id:guid}/deliver")]
    public async Task<ActionResult<OrderResponse>> Deliver(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderService.DeliverAsync(
            id,
            cancellationToken);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<OrderResponse>> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderService.CancelAsync(
            id,
            cancellationToken);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }
}