using FStore.Catalog.Application.Features.Categories.DTOs;
using MediatR;

namespace FStore.Catalog.Application.Features.Categories.Queries.GetCategories;

public record GetCategoriesQuery : IRequest<List<CategoryDto>>;