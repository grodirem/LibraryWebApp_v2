using BLL.DTOs.Requests;
using FluentValidation;

namespace BLL.Validators;

public class UpdateBookDtoValidator : AbstractValidator<UpdateBookDto>
{
    public UpdateBookDtoValidator()
    {
        RuleFor(x => x).NotNull().WithMessage("Данные книги не могут быть null");

        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id книги должно быть больше 0");

        RuleFor(x => x.AuthorId)
            .GreaterThan(0).WithMessage("Id автора должно быть больше 0");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Название книги обязательно")
            .MaximumLength(100).WithMessage("Название не должно превышать 100 символов");
    }
}
