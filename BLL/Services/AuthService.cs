using AutoMapper;
using BLL.DTOs.Requests;
using BLL.DTOs.Responses;
using DAL.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BLL.Services;

public class AuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly TokenService _tokenService;

    public AuthService(UserManager<User> userManager, IConfiguration configuration, IMapper mapper, TokenService tokenService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> AuthAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null)
        {
            return new AuthResponseDto
            {
                IsAuthenticated = false,
                ErrorMessage = "Пользователь не найден."
            };
        }

        if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            return new AuthResponseDto
            {
                IsAuthenticated = false,
                ErrorMessage = "Неверный пароль."
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateJwtToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        var response = _mapper.Map<AuthResponseDto>(user);
        response.Token = token;
        response.RefreshToken = refreshToken;
        response.IsAuthenticated = true;
        return response;
    }

    public async Task<RegistrationResponseDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        if (registerDto == null)
        {
            throw new ValidationException("Необходимо ввести данные пользователя.");
        }

        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);

        if (existingUser != null)
        {
            return new RegistrationResponseDto
            {
                Errors = new List<string> { "Пользователь с таким email уже существует." }
            };
        }

        var user = _mapper.Map<User>(registerDto);
        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return new RegistrationResponseDto { Errors = errors };
        }

        await _userManager.AddToRoleAsync(user, "User");
        return new RegistrationResponseDto { IsRegistered = true };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken, cancellationToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return new AuthResponseDto
            {
                ErrorMessage = "Токен обновления недействителен."
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _tokenService.GenerateJwtToken(user, roles);

        return new AuthResponseDto
        {
            IsAuthenticated = true,
            Token = newAccessToken,
            RefreshToken = request.RefreshToken
        };
    }

    public async Task LogoutAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.GetUserAsync(principal);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Пользователь не найден.");
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await _userManager.UpdateAsync(user);
    }
}