using BLL.DTOs.Requests;
using FluentValidation;

namespace BLL.Validators;

public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>
{
    public CreateBookDtoValidator()
    {
        RuleFor(b => b).NotNull().WithMessage("Данные книги не могут быть null");

        RuleFor(b => b.AuthorId)
            .GreaterThan(0).WithMessage("Id автора должно быть больше 0");

        RuleFor(b => b.Title)
            .NotEmpty().WithMessage("Название книги обязательно")
            .MaximumLength(100).WithMessage("Название не должно превышать 100 символов");

        RuleFor(b => b.ISBN)
            .MaximumLength(20).WithMessage("ISBN не должен превышать 20 символов")
            .When(x => !string.IsNullOrEmpty(x.ISBN));

        RuleFor(b => b.Image)
            .Must(image => image == null || image.Length <= 5 * 1024 * 1024)
            .WithMessage("Размер изображения не должен превышать 5MB");
    }
}

