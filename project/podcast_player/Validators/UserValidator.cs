using FluentValidation;
using Project.Models;

namespace Project.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Неверный формат email")
            .MaximumLength(256).WithMessage("Email не должен превышать 256 символов");

        RuleFor(u => u.PasswordHash)
            .NotEmpty().WithMessage("Пароль обязателен")
            .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов");

        RuleFor(u => u.FirstName)
            .NotEmpty().WithMessage("Имя обязательно")
            .MaximumLength(100).WithMessage("Имя не должно превышать 100 символов");

        RuleFor(u => u.LastName)
            .MaximumLength(100).WithMessage("Фамилия не должна превышать 100 символов");

        RuleFor(u => u.DefaultPlaybackSpeed)
            .GreaterThan(0).WithMessage("Скорость воспроизведения должна быть больше 0")
            .LessThanOrEqualTo(3.0).WithMessage("Скорость воспроизведения не должна превышать 3.0");
    }
}

