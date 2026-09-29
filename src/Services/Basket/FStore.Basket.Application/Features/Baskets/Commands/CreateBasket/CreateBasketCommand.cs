using MediatR;

namespace FStore.Basket.Application.Features.Baskets.Commands.CreateBasket;

public record CreateBasketCommand(Guid UserId, List<BasketItemDto> Items) : IRequest<IRequest<bool>>;

public record BasketItemDto(Guid ProductId, string ProductName, decimal Price, int Quantity);