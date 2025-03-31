using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Requests;

public class LoginDto
{
    [Required(ErrorMessage = "Введите Email.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Введите пароль.")]
    public string? Password { get; set; }
}
