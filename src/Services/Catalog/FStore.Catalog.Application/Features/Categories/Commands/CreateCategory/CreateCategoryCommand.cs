using MediatR;

namespace FStore.Catalog.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name) : IRequest<Guid>;