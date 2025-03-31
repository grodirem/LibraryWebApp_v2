namespace BLL.DTOs.Responses;

public class RegistrationResponseDto
{
    public bool IsRegistered { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}
