namespace TaskPulse.Api.DTOs.Tasks;

public class TaskResponseDto
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public Guid? AssignedToUserId { get; set; }

    public Guid CreatedByUserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? AssignedToUserName { get; set; }

    public string? CreatedByUserName { get; set; }
}