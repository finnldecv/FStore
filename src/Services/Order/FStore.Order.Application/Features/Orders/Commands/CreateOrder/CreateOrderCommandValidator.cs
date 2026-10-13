using FluentValidation;

namespace FStore.Order.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
  public CreateOrderValidator()
  {
    RuleFor(x => x.UserId).NotEmpty();
    RuleFor(x => x.ShippingAddress).NotEmpty().MaximumLength(500);
    RuleFor(x => x.Items).NotEmpty().WithMessage("Order must contain at least one item.");
    RuleForEach(x => x.Items).ChildRules(item =>
    {
      item.RuleFor(i => i.ProductId).NotEmpty();
      item.RuleFor(i => i.ProductName).NotEmpty();
      item.RuleFor(i => i.UnitPrice).GreaterThanOrEqualTo(0);
      item.RuleFor(i => i.Quantity).GreaterThan(0);
    });
  }
}