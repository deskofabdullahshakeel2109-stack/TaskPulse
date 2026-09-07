using Microsoft.AspNetCore.Identity;
using TaskPulse.Api.DTOs.Users;
using TaskPulse.Api.Models;
using TaskPulse.Api.Repositories.Interfaces;

namespace TaskPulse.Api.Services.Users;

public class UserService(
    IUserRepository repository,
    IPasswordHasher<User> passwordHasher) : IUserService
{
    public async Task<List<UserResponseDto>> GetAllAsync() =>
        (await repository.GetAllAsync())
            .Select(Map)
            .ToList();

    public async Task<UserResponseDto?> GetByIdAsync(Guid id)
    {
        var user = await repository.GetByIdAsync(id);

        return user is null ? null : Map(user);
    }

    public async Task<UserResponseDto?> GetByEmailAsync(string email)
    {
        var user = await repository.GetByEmailAsync(
            email.Trim().ToLowerInvariant());

        return user is null ? null : Map(user);
    }

    public async Task<UserResponseDto> CreateAsync(CreateUserDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        // Prevent duplicate email addresses.
        if (await repository.EmailExistsAsync(email))
            throw new InvalidOperationException("Email is already in use.");

        // Only allow valid TaskPulse roles.
        var allowedRoles = new[] { "Admin", "Manager", "Member" };

        var role = dto.Role.Trim();

        if (!allowedRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "Invalid role. Allowed roles are Admin, Manager, and Member.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = dto.FullName.Trim(),
            Email = email,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        // Securely hash the password before saving it.
        user.PasswordHash = passwordHasher.HashPassword(
            user,
            dto.Password);

        await repository.CreateAsync(user);

        return Map(user);
    }

    public async Task<UserResponseDto?> UpdateAsync(
        Guid id,
        UpdateUserDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var duplicate = await repository.GetByEmailAsync(email);

        if (duplicate is not null && duplicate.Id != id)
            throw new InvalidOperationException("Email is already in use.");

        var existing = await repository.GetByIdAsync(id);

        if (existing is null)
            return null;

        existing.FullName = dto.FullName.Trim();
        existing.Email = email;
        existing.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateAsync(existing);

        return Map(existing);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await repository.GetByIdAsync(id);

        if (user is null)
            return false;

        await repository.DeleteAsync(user);

        return true;
    }

    private static UserResponseDto Map(User u) => new()
    {
        Id = u.Id,
        FullName = u.FullName,
        Email = u.Email,
        Role = u.Role,
        CreatedAt = u.CreatedAt,
        UpdatedAt = u.UpdatedAt
    };
}