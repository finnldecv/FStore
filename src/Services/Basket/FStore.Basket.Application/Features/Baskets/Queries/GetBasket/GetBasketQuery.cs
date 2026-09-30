using FStore.Basket.Domain.Entities;
using MediatR;

namespace FStore.Basket.Application.Features.Baskets.Queries.GetBasket;
public record GetBasketQuery(Guid UserId): IRequest<ShoppingCart?>;