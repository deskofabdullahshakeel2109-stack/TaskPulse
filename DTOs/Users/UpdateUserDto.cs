using System.ComponentModel.DataAnnotations;

namespace TaskPulse.Api.DTOs.Users;

public class UpdateUserDto
{
    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; } = string.Empty;
}
