using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TaskPulse.Api.DTOs.Auth;
using TaskPulse.Api.DTOs.Users;
using TaskPulse.Api.Models;
using TaskPulse.Api.Repositories.Interfaces;

namespace TaskPulse.Api.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository users, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        if (await _users.EmailExistsAsync(email))
            throw new InvalidOperationException("Email is already registered.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = dto.FullName.Trim(),
            Email = email,
            Role = "Member",
            CreatedAt = DateTime.UtcNow
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
        await _users.CreateAsync(user);

        return new AuthResponseDto
        {
            Token = CreateToken(user),
            User = Map(user)
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _users.GetByEmailAsync(dto.Email.Trim().ToLowerInvariant());
        if (user is null) return null;

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed) return null;

        return new AuthResponseDto
        {
            Token = CreateToken(user),
            User = Map(user)
        };
    }

    public async Task<UserResponseDto?> GetMeAsync(Guid userId)
    {
        var user = await _users.GetByIdAsync(userId);
        return user is null ? null : Map(user);
    }

    private string CreateToken(User user)
    {
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured.");
        var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is not configured.");
        var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience is not configured.");
        var minutes = _configuration.GetValue<int?>("Jwt:ExpirationMinutes") ?? 60;

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer, audience, claims,
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static UserResponseDto Map(User u) => new()
    {
        Id = u.Id, FullName = u.FullName, Email = u.Email, Role = u.Role,
        CreatedAt = u.CreatedAt, UpdatedAt = u.UpdatedAt
    };
}
