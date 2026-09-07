using TaskPulse.Api.Models;

namespace TaskPulse.Api.Repositories.Interfaces;

public interface ITaskRepository
{
    Task<List<TaskItem>> GetAllAsync();

    Task<List<TaskItem>> GetByProjectIdAsync(Guid projectId);

    Task<(List<TaskItem> Items, int TotalCount)> GetFilteredAsync(
        Guid projectId,
        string? search,
        string? status,
        string? priority,
        Guid? assignedToUserId,
        string sortBy,
        string sortOrder,
        int page,
        int pageSize);

    Task<TaskItem?> GetByIdAsync(Guid id);

    Task<TaskItem> CreateAsync(TaskItem task);

    Task<TaskItem?> UpdateAsync(TaskItem task);

    Task<bool> DeleteAsync(Guid id);
}