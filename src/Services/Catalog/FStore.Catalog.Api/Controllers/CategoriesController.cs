using FStore.Catalog.Application.Features.Categories.Commands.CreateCategory;
using FStore.Catalog.Application.Features.Categories.Commands.DeleteCategory;
using FStore.Catalog.Application.Features.Categories.Commands.UpdateCategory;
using FStore.Catalog.Application.Features.Categories.Queries.GetCategories;
using FStore.Catalog.Application.Features.Categories.Queries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FStore.Catalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
  private readonly IMediator _mediator;

  public CategoriesController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    var result =await _mediator.Send(new GetCategoriesQuery());
    return Ok(result);
  }

  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    var result = await _mediator.Send(new GetCategoryByIdQuery(id));
    return result is null ? NotFound() : Ok(result);
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
  {
    var id = await _mediator.Send(command);
    return CreatedAtAction(nameof(GetById), new { id }, id);
  }

  [HttpPut("{id:guid}")]
  public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command)
  {
    if (id != command.Id) return BadRequest("Id mismatch");
    var updated = await _mediator.Send(command);
    return updated ? NoContent() : NotFound();
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> Delete(Guid id)
  {
    var deleted = await _mediator.Send(new DeleteCategoryCommand(id));
    return deleted ? NoContent() : NotFound();
  }
}