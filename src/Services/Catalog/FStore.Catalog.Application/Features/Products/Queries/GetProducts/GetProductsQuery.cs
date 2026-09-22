using FStore.Catalog.Application.Features.Products.DTOs;
using MediatR;

namespace FStore.Catalog.Application.Features.Products.Queries.GetProducts;

public record GetProductsQuery : IRequest<List<ProductDto>>;