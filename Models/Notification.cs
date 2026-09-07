namespace TaskPulse.Api.Models;

public class Notification
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid? ProjectId { get; set; }

    public Guid? TaskId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;

    public Project? Project { get; set; }

    public TaskItem? Task { get; set; }
}