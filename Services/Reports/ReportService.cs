using TaskPulse.Api.DTOs.Reports;
using TaskPulse.Api.Repositories.Interfaces;

namespace TaskPulse.Api.Services.Reports;

public class ReportService(
    IReportRepository repository) : IReportService
{
    public async Task<ProjectReportResponseDto> GetProjectReportAsync(
        Guid projectId)
    {
        var totalTasks =
            await repository.GetTotalTasksAsync(projectId);

        var completedTasks =
            await repository.GetCompletedTasksAsync(projectId);

        var pendingTasks =
            await repository.GetPendingTasksAsync(projectId);

        var overdueTasks =
            await repository.GetOverdueTasksAsync(projectId);

        var unassignedTasks =
            await repository.GetUnassignedTasksAsync(projectId);

        var tasksByStatus =
            await repository.GetTasksByStatusAsync(projectId);

        var tasksByPriority =
            await repository.GetTasksByPriorityAsync(projectId);

        var tasksByAssignee =
            await repository.GetTasksByAssigneeAsync(projectId);

        var completionPercentage = totalTasks == 0
            ? 0
            : Math.Round(
                (double)completedTasks / totalTasks * 100,
                2);

        return new ProjectReportResponseDto
        {
            ProjectId = projectId,
            TotalTasks = totalTasks,
            CompletedTasks = completedTasks,
            PendingTasks = pendingTasks,
            OverdueTasks = overdueTasks,
            UnassignedTasks = unassignedTasks,
            CompletionPercentage = completionPercentage,
            TasksByStatus = tasksByStatus,
            TasksByPriority = tasksByPriority,
            TasksByAssignee = tasksByAssignee
        };
    }
}