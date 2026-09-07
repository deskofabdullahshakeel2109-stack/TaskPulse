namespace TaskPulse.Api.Services.CurrentUser;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Role { get; }
    bool IsAdmin { get; }
}
