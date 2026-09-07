namespace TaskPulse.Api.Models;

public class Comment
{
    public Guid Id { get; set; }

    public Guid TaskId { get; set; }

    public Guid UserId { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public TaskItem Task { get; set; } = null!;

    public User User { get; set; } = null!;
}