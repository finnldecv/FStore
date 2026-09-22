using FStore.Catalog.Application.Features.Products.Commands.CreateProduct;
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

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
  {
    var id = await _mediator.Send(command);

    return CreatedAtAction(nameof(GetAll), new { id }, id);
  }
}