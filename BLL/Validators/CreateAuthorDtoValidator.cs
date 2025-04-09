using BLL.DTOs.Models;
using BLL.DTOs.Requests;
using FluentValidation;
using FluentValidation.Results;

namespace BLL.Validators;

public class CreateAuthorDtoValidator : AbstractValidator<CreateAuthorDto>
{
    public CreateAuthorDtoValidator()
    {
        RuleFor(a => a.FirstName)
         .NotEmpty().WithMessage("Введите имя.")
         .MaximumLength(30).WithMessage("Имя не должно превышать 30 символов.");

        RuleFor(author => author.LastName)
            .NotEmpty().WithMessage("Введите фамилию.")
            .MaximumLength(30).WithMessage("Фамилия не должна превышать 30 символов.");

        RuleFor(author => author.BirthDate)
            .NotEmpty().WithMessage("Введите дату рождения.")
            .LessThan(DateTime.Today).WithMessage("Дата рождения должна быть в прошлом.");

        RuleFor(author => author.Country)
            .NotEmpty().WithMessage("Введите страну.");
    }
}
