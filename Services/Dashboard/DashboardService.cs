using TaskPulse.Api.DTOs.Dashboard;
using TaskPulse.Api.Repositories.Interfaces;

namespace TaskPulse.Api.Services.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardService(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<DashboardResponseDto> GetProjectDashboardAsync(
        Guid projectId)
    {
        var totalTasks =
            await _dashboardRepository.GetTotalTasksAsync(projectId);

        var completedTasks =
            await _dashboardRepository.GetCompletedTasksAsync(projectId);

        var pendingTasks =
            await _dashboardRepository.GetPendingTasksAsync(projectId);

        var overdueTasks =
            await _dashboardRepository.GetOverdueTasksAsync(projectId);

        var unassignedTasks =
            await _dashboardRepository.GetUnassignedTasksAsync(projectId);

        var tasksByStatus =
            await _dashboardRepository.GetTasksByStatusAsync(projectId);

        var tasksByPriority =
            await _dashboardRepository.GetTasksByPriorityAsync(projectId);

        var completionPercentage = totalTasks == 0
            ? 0
            : Math.Round(
                (double)completedTasks / totalTasks * 100,
                2);

        return new DashboardResponseDto
        {
            TotalTasks = totalTasks,
            CompletedTasks = completedTasks,
            PendingTasks = pendingTasks,
            OverdueTasks = overdueTasks,
            UnassignedTasks = unassignedTasks,
            CompletionPercentage = completionPercentage,
            TasksByStatus = tasksByStatus,
            TasksByPriority = tasksByPriority
        };
    }
}