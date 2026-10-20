using MediatR;

namespace FStore.Basket.Application.Features.Baskets.Commands.CheckoutBasket;

public record CheckoutBasketCommand(
  Guid UserId,
  string ShippingAddress,
  string? Notes
) : IRequest<bool>;