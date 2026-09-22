using FluentValidation;

namespace FStore.Catalog.Application.Features.Products.Commands.CreateProduct;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
  public CreateProductValidator()
  {
    RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    RuleFor(x => x.Price).GreaterThan(0);
    RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
  }
}