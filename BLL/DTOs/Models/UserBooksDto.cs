namespace BLL.DTOs.Models;

public class UserBooksDto
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public DateTime? BorrowedAt { get; set; }
    public DateTime? ReturnedBy { get; set; }
}
