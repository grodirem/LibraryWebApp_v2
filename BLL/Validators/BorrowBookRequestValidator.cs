using BLL.DTOs.Requests;
using FluentValidation;

namespace BLL.Validators;

public class BorrowBookRequestValidator : AbstractValidator<BorrowBookRequest>
{
    public BorrowBookRequestValidator()
    {
        RuleFor(b => b.BookId).GreaterThan(0).WithMessage("Id должен быть больше 0.");
        RuleFor(b => b.ReturnBy).GreaterThan(DateTime.UtcNow).WithMessage("Дата возврата должна быть в будущем");
    }
}