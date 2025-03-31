using BLL.DTOs.Models;
using FluentValidation;
using FluentValidation.Results;

namespace BLL.Validators;

public interface IAuthorDtoValidator
{
    Task<ValidationResult> ValidateAsync(AuthorDto authorDto, CancellationToken cancellationToken = default);
}

public class AuthorDtoValidator : AbstractValidator<AuthorDto>, IAuthorDtoValidator
{
    public AuthorDtoValidator()
    {
        RuleFor(author => author.FirstName)
            .NotEmpty().WithMessage("Введите имя.")
            .Length(1, 30).WithMessage("Имя не должно превышать 30 символов.");

        RuleFor(author => author.LastName)
            .NotEmpty().WithMessage("Введите фамилию.")
            .Length(1, 30).WithMessage("Фамилия не должна превышать 30 символов.");

        RuleFor(author => author.BirthDate)
            .NotEmpty().WithMessage("Введите дату рождения.")
            .LessThan(DateTime.Now).WithMessage("Дата рождения должна быть в прошлом.");

        RuleFor(author => author.Country)
            .NotEmpty().WithMessage("Введите страну.");
    }
}