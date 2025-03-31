namespace BLL.DTOs.Requests;

public class BorrowBookRequest
{
    public int BookId { get; set; }
    public DateTime ReturnBy { get; set; }
}
