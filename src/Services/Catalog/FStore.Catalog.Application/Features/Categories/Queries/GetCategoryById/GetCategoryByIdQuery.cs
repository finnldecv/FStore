using FStore.Catalog.Application.Features.Categories.DTOs;
using FStore.Catalog.Domain.Entities;
using MediatR;

namespace FStore.Catalog.Application.Features.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto?>;