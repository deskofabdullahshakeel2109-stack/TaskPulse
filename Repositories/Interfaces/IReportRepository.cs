using TaskPulse.Api.Models;

namespace TaskPulse.Api.Repositories.Interfaces;

public interface IReportRepository
{
    Task<int> GetTotalTasksAsync(Guid projectId);

    Task<int> GetCompletedTasksAsync(Guid projectId);

    Task<int> GetPendingTasksAsync(Guid projectId);

    Task<int> GetOverdueTasksAsync(Guid projectId);

    Task<int> GetUnassignedTasksAsync(Guid projectId);

    Task<Dictionary<string, int>> GetTasksByStatusAsync(
        Guid projectId);

    Task<Dictionary<string, int>> GetTasksByPriorityAsync(
        Guid projectId);

    Task<Dictionary<string, int>> GetTasksByAssigneeAsync(
        Guid projectId);
}