using TaskPulse.Api.DTOs.Users;

namespace TaskPulse.Api.Services.Users;

public interface IUserService
{
    Task<List<UserResponseDto>> GetAllAsync();

    Task<UserResponseDto?> GetByIdAsync(Guid id);

    Task<UserResponseDto?> GetByEmailAsync(string email);

    Task<UserResponseDto> CreateAsync(CreateUserDto dto);

    Task<UserResponseDto?> UpdateAsync(Guid id, UpdateUserDto dto);

    Task<bool> DeleteAsync(Guid id);
}