using FluentValidation;

namespace FStore.Basket.Application.Features.Baskets.Commands.CheckoutBasket;

public class CheckoutBasketValidator : AbstractValidator<CheckoutBasketCommand>
{
  public CheckoutBasketValidator()
  {
    RuleFor(x => x.UserId).NotEmpty();
    RuleFor(x => x.ShippingAddress).NotEmpty().MaximumLength(500);
    RuleFor(x => x.Notes).MaximumLength(2000);
  }
}