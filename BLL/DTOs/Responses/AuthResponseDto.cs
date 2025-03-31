namespace BLL.DTOs.Responses;

public class AuthResponseDto
{
    public bool IsAuthenticated { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
}
