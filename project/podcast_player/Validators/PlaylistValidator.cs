using FluentValidation;
using Project.Models;

namespace Project.Validators;

public class PlaylistValidator : AbstractValidator<Playlist>
{
    public PlaylistValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Название плейлиста обязательно")
            .MaximumLength(255).WithMessage("Название не должно превышать 255 символов");

        RuleFor(p => p.Description)
            .MaximumLength(1000).WithMessage("Описание не должно превышать 1000 символов");

        RuleFor(p => p.OwnerId)
            .GreaterThan(0).WithMessage("ID владельца должен быть больше 0");
    }
}

