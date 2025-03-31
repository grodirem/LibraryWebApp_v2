using AutoMapper;
using BLL.DTOs.Requests;
using BLL.DTOs.Responses;
using DAL.Models;
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

    public AuthService(UserManager<User> userManager, IConfiguration configuration, IMapper mapper)
    {
        _userManager = userManager;
        _configuration = configuration;
        _mapper = mapper;
    }

    public async Task<AuthResponseDto> AuthAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password)) 
        {
            return new AuthResponseDto();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        var response = _mapper.Map<AuthResponseDto>(user);
        response.Token = token;
        response.RefreshToken = refreshToken;
        response.IsAuthenticated = true;
        return response;
    }

    public async Task<RegistrationResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        if (registerDto == null)
        {
            throw new ArgumentNullException(nameof(registerDto), "Необходимо ввести данные пользователя.");
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

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return new AuthResponseDto { ErrorMessage = "Токен обновления недействителен." };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = GenerateJwtToken(user, roles);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            IsAuthenticated = true,
            Token = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task LogoutAsync(ClaimsPrincipal principal)
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

    private string GenerateJwtToken(User user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:securityKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:expiryInMinutes"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:validIssuer"],
            audience: _configuration["JwtSettings:validAudience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var rand = new byte[32];

        using (var randGenerator = RandomNumberGenerator.Create())
        {
            randGenerator.GetBytes(rand);
            return Convert.ToBase64String(rand);
        }
    }
}
