using FluentValidation;

namespace FStore.Basket.Application.Features.Baskets.Commands.UpdateBasket;

public class UpdateBasketValidator : AbstractValidator<UpdateBasketCommand>
{
  public UpdateBasketValidator()
  {
    RuleFor(x => x.UserId).NotEmpty();
    RuleForEach(x => x.Items).ChildRules(item =>
    {
      item.RuleFor(i => i.ProductId).NotEmpty();
      item.RuleFor(i => i.Quantity).GreaterThan(0);
      item.RuleFor(i => i.Price).GreaterThanOrEqualTo(0);
    });
  }
}