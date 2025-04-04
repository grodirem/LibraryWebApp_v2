using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Requests;

public class LoginDto
{
    [Required(ErrorMessage = "Введите Email.")]
    [EmailAddress(ErrorMessage = "Неверный формат Email адреса.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Введите пароль.")]
    public string? Password { get; set; }
}
