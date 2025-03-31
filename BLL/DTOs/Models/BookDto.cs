namespace BLL.DTOs.Models;

public class BookDto
{
    public int Id { get; set; }
    public string AuthorName { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? BorrowedAt { get; set; }
    public DateTime? ReturnBy { get; set; }
    public bool IsBorrowed { get; set; }
    public string? ImagePath { get; set; }
}
