using TaskPulse.Api.DTOs.ActivityLogs;

namespace TaskPulse.Api.Services.ActivityLogs;

public interface IActivityLogService
{
    Task<List<ActivityLogResponseDto>> GetByProjectIdAsync(
        Guid projectId);

    Task CreateAsync(
        Guid projectId,
        Guid userId,
        string action,
        string description);
}