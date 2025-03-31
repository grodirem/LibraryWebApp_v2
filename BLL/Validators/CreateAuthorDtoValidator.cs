using BLL.DTOs.Requests;
using DAL.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace BLL.Validators;

public class CreateAuthorDtoValidator : AbstractValidator<CreateAuthorDto>
{
    private readonly IAuthorRepository _authorRepository;

    public CreateAuthorDtoValidator(IAuthorRepository authorRepository)
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

        RuleFor(author => author)
            .MustAsync(BeUniqueAuthor).WithMessage("Автор уже существует.");
    }

    public Task<ValidationResult> ValidateDtoAsync(CreateAuthorDto authorDto, CancellationToken cancellationToken = default)
    {
        return ValidateAsync(authorDto, cancellationToken);
    }

    private async Task<bool> BeUniqueAuthor(CreateAuthorDto authorDto, CancellationToken cancellationToken)
    {
        var existingAuthor = await _authorRepository.GetByNameAsync($"{authorDto.FirstName} {authorDto.LastName}");
        return existingAuthor == null;
    }
}
