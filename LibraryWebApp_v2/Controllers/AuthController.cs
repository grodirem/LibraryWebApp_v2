using BLL.DTOs.Requests;
using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryWebApp_v2.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        var result = await _authService.AuthAsync(loginDto, cancellationToken);
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto userDto, CancellationToken cancellationToken = default)
    {
        var result = await _authService.RegisterAsync(userDto, cancellationToken);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _authService.RefreshTokenAsync(request, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
    {
        await _authService.LogoutAsync(User, cancellationToken);
        return Ok();
    }
}