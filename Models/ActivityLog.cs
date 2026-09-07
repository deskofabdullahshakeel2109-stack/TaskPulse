namespace TaskPulse.Api.Models;

public class ActivityLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? ProjectId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public User User { get; set; } = null!;
    public Project? Project { get; set; }
}
