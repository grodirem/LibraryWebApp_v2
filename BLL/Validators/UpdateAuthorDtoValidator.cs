using BLL.DTOs.Models;
using BLL.DTOs.Requests;
using DAL.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace BLL.Validators;

public interface IUpdateAuthorDtoValidator
{
    Task<ValidationResult> ValidateAsync(UpdateAuthorDto updateAuthorDto, CancellationToken cancellationToken = default);
}

public class UpdateAuthorDtoValidator : AbstractValidator<UpdateAuthorDto>, IUpdateAuthorDtoValidator
{
    private readonly IAuthorRepository _authorRepository;

    public UpdateAuthorDtoValidator(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;

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

    public Task<ValidationResult> ValidateDtoAsync(UpdateAuthorDto authorDto, CancellationToken cancellationToken = default)
    {
        return ValidateAsync(authorDto, cancellationToken);
    }
}