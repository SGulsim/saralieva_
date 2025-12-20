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

        RuleFor(p => p.RssFeedUrl)
            .NotEmpty().WithMessage("RSS-ссылка обязательна")
            .MaximumLength(500).WithMessage("RSS-ссылка не должна превышать 500 символов")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("RSS-ссылка должна быть валидным URL");

        RuleFor(p => p.Description)
            .MaximumLength(2000).WithMessage("Описание не должно превышать 2000 символов");

        RuleFor(p => p.CoverImageUrl)
            .MaximumLength(500).WithMessage("Ссылка на обложку не должна превышать 500 символов")
            .Must(url => string.IsNullOrEmpty(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Ссылка на обложку должна быть валидным URL");

        RuleFor(p => p.Language)
            .MaximumLength(10).WithMessage("Язык не должен превышать 10 символов");
    }
}

