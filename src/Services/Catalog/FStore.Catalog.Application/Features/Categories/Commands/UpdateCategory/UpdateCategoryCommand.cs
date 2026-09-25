using MediatR;

namespace FStore.Catalog.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(Guid Id, string Name) : IRequest<bool>;