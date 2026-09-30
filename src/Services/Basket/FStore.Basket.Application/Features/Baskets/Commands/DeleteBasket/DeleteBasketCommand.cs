using MediatR;

namespace FStore.Basket.Application.Features.Baskets.Commands.DeleteBasket;

public record DeleteBasketCommand(Guid UserId): IRequest<bool>;