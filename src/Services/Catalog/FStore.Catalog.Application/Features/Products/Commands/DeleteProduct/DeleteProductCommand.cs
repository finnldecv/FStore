using MediatR;

namespace FStore.Catalog.Application.Features.Products.Commands.DeleteProduct;

public record  DeleteProductCommand(Guid Id) : IRequest<bool>;