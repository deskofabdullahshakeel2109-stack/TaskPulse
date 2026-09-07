using TaskPulse.Api.DTOs.Users;

namespace TaskPulse.Api.DTOs.Auth;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserResponseDto User { get; set; } = null!;
}
