using MediatR;

namespace FStore.Basket.Application.Features.Baskets.Commands.UpdateBasket;

public record UpdateBasketCommand(Guid UserId, List<UpdateBasketItemDto> Items): IRequest<bool>;

public record UpdateBasketItemDto(Guid ProductId, string ProductName, decimal Price, int Quantity);