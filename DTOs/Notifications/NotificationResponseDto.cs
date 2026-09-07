namespace TaskPulse.Api.DTOs.Notifications;

public class NotificationResponseDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid? ProjectId { get; set; }

    public Guid? TaskId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }
}