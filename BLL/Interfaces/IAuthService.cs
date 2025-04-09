using BLL.DTOs.Requests;
using BLL.DTOs.Responses;
using System.Security.Claims;

namespace BLL.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> AuthAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
    Task<RegistrationResponseDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    Task LogoutAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default);
}
