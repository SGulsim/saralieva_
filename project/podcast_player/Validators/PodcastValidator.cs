using FluentValidation;
using Project.Models;

namespace Project.Validators;

public class PodcastValidator : AbstractValidator<Podcast>
{
    public PodcastValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Название подкаста обязательно")
            .MaximumLength(255).WithMessage("Название не должно превышать 255 символов");

        RuleFor(p => p.Author)
            .NotEmpty().WithMessage("Автор подкаста обязателен")
            .MaximumLength(255).WithMessage("Имя автора не должно превышать 255 символов");
    }
}

