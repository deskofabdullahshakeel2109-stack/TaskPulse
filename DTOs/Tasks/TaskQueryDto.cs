namespace TaskPulse.Api.DTOs.Tasks;

public class TaskQueryDto
{
    public string? Search { get; set; }

    public string? Status { get; set; }

    public string? Priority { get; set; }

    public Guid? AssignedToUserId { get; set; }

    public string? SortBy { get; set; } = "CreatedAt";

    public string? SortOrder { get; set; } = "desc";

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}