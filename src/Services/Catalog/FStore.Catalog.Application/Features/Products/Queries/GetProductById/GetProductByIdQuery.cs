using FStore.Catalog.Application.Features.Products.DTOs;
using MediatR;

namespace FStore.Catalog.Application.Features.Categories.Queries.GetCategoryById;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;