using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Requests;

public class RegisterDto
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    [Required(ErrorMessage = "Введите Email.")]
    [EmailAddress(ErrorMessage = "Неверный формат Email адреса.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Введите пароль.")]
    public string? Password { get; set; }


    [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
    public string? ConfirmPassword { get; set; }
}
