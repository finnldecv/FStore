using FluentValidation;

namespace FStore.Catalog.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
  public CreateCategoryValidator()
  {
    RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
  }
}