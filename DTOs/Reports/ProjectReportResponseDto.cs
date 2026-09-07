namespace TaskPulse.Api.DTOs.Reports;

public class ProjectReportResponseDto
{
    public Guid ProjectId { get; set; }

    public int TotalTasks { get; set; }

    public int CompletedTasks { get; set; }

    public int PendingTasks { get; set; }

    public int OverdueTasks { get; set; }

    public int UnassignedTasks { get; set; }

    public double CompletionPercentage { get; set; }

    public Dictionary<string, int> TasksByStatus { get; set; } = new();

    public Dictionary<string, int> TasksByPriority { get; set; } = new();

    public Dictionary<string, int> TasksByAssignee { get; set; } = new();
}