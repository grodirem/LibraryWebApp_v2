using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Requests;

public class UpdateBookDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите автора.")]
    public int AuthorId { get; set; }

    [Required(ErrorMessage = "Введите ISBN.")]
    [StringLength(13, ErrorMessage = "ISBN не должен превышать 13 символов.")]
    public string ISBN { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите название.")]
    [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(30, ErrorMessage = "Название жанра не должно превышать 30 символов.")]
    public string Genre { get; set; } = string.Empty;

    [StringLength(400, ErrorMessage = "Описание не должно превышать 400 символов.")]
    public string Description { get; set; } = string.Empty;

    public IFormFile? Image { get; set; }
}
