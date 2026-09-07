using TaskPulse.Api.DTOs.Auth;
using TaskPulse.Api.DTOs.Users;

namespace TaskPulse.Api.Services.Auth;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<UserResponseDto?> GetMeAsync(Guid userId);
}
