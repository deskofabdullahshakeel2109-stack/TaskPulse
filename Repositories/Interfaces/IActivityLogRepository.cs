using TaskPulse.Api.Models;

namespace TaskPulse.Api.Repositories.Interfaces;

public interface IActivityLogRepository
{
    Task<ActivityLog> CreateAsync(ActivityLog log);

    Task<List<ActivityLog>> GetByProjectIdAsync(
        Guid projectId);
}