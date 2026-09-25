using FluentValidation;

namespace FStore.Catalog.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
  public UpdateProductValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    RuleFor(x => x.Price).GreaterThan(0);
    RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
  }
}