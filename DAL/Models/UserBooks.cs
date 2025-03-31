namespace DAL.Models;

public class UserBooks
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string UserId { get; set; } = string.Empty;
}
