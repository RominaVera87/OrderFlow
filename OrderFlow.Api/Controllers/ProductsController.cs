using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.DTOs.Products;
using OrderFlow.Application.Interfaces;

namespace OrderFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var products = await _productService.GetAllAsync(
            cancellationToken);

        return Ok(products);
    }

    [HttpGet("in-stock")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetInStock(
        CancellationToken cancellationToken)
    {
        var products = await _productService.GetInStockAsync(
            cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(
            id,
            cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _productService.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> Update(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _productService.UpdateAsync(
            id,
            request,
            cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _productService.DeleteAsync(id,cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}