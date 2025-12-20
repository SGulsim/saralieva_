using FluentValidation;
using Project.Models;

namespace Project.Validators;

public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Название категории обязательно")
            .MaximumLength(100).WithMessage("Название не должно превышать 100 символов");

        RuleFor(c => c.Icon)
            .MaximumLength(100).WithMessage("Иконка не должна превышать 100 символов");
    }
}

