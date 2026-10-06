using FStore.Catalog.Application.Features.Categories.Queries.GetCategoryById;
using FStore.Catalog.Application.Features.Products.Commands.CreateProduct;
using FStore.Catalog.Application.Features.Products.Commands.DeleteProduct;
using FStore.Catalog.Application.Features.Products.Commands.UpdateProduct;
using FStore.Catalog.Application.Features.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FStore.Catalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
  private readonly IMediator _mediator;
  public ProductsController(IMediator mediator) => _mediator = mediator;

  [HttpGet]
  public async Task<IActionResult> GetAll()
    => Ok(await _mediator.Send(new GetProductsQuery()));

  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    var result = await _mediator.Send(new GetProductByIdQuery(id));
    return result is null ? NotFound() : Ok(result);
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
  {
    var product = await _mediator.Send(command);

    return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
  }
  [HttpPut("{id:guid}")]
  public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
  {
    if (id != command.Id) return BadRequest("Route id and body id do not match.");
    var updated = await _mediator.Send(command);
    return updated ? NoContent() : NotFound();
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> Delete(Guid id)
  {
    var deleted = await _mediator.Send(new DeleteProductCommand(id));
    return deleted ? NoContent() : NotFound();
  }
}