using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Requests;

public class UpdateAuthorDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите имя.")]
    [StringLength(30, ErrorMessage = "Имя не должно превышать 30 символов.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите фамилию.")]
    [StringLength(30, ErrorMessage = "Фамилия не должна превышать 30 символов.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите дату рождения.")]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "Введите страну.")]
    public string Country { get; set; } = string.Empty;
}
