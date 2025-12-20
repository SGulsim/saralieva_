using FluentValidation;
using Project.Models;

namespace Project.Validators;

public class EpisodeValidator : AbstractValidator<Episode>
{
    public EpisodeValidator()
    {
        RuleFor(e => e.Title)
            .NotEmpty().WithMessage("Название эпизода обязательно")
            .MaximumLength(500).WithMessage("Название не должно превышать 500 символов");

        RuleFor(e => e.Description)
            .MaximumLength(5000).WithMessage("Описание не должно превышать 5000 символов");

        RuleFor(e => e.AudioFileUrl)
            .NotEmpty().WithMessage("Ссылка на аудиофайл обязательна")
            .MaximumLength(1000).WithMessage("Ссылка на аудиофайл не должна превышать 1000 символов")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Ссылка на аудиофайл должна быть валидным URL");

        RuleFor(e => e.PodcastId)
            .GreaterThan(0).WithMessage("ID подкаста должен быть больше 0");

        RuleFor(e => e.DurationInSeconds)
            .GreaterThanOrEqualTo(0).WithMessage("Длительность не может быть отрицательной");

        RuleFor(e => e.ProgressInSeconds)
            .GreaterThanOrEqualTo(0).WithMessage("Прогресс не может быть отрицательным");

        RuleFor(e => e.FileSizeInBytes)
            .GreaterThanOrEqualTo(0).When(e => e.FileSizeInBytes.HasValue)
            .WithMessage("Размер файла не может быть отрицательным");
    }
}

