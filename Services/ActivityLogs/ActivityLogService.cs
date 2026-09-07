using TaskPulse.Api.DTOs.ActivityLogs;
using TaskPulse.Api.Models;
using TaskPulse.Api.Repositories.Interfaces;

namespace TaskPulse.Api.Services.ActivityLogs;

public class ActivityLogService : IActivityLogService
{
    private readonly IActivityLogRepository _activityLogRepository;

    public ActivityLogService(
        IActivityLogRepository activityLogRepository)
    {
        _activityLogRepository = activityLogRepository;
    }

    public async Task<List<ActivityLogResponseDto>> GetByProjectIdAsync(
        Guid projectId)
    {
        var logs = await _activityLogRepository
            .GetByProjectIdAsync(projectId);

        return logs.Select(log => new ActivityLogResponseDto
        {
            Id = log.Id,
            ProjectId = log.ProjectId ?? Guid.Empty,
            UserId = log.UserId,
            UserName = log.User?.FullName ?? string.Empty,
            Action = log.Action,
            Description = log.Description ?? string.Empty,
            CreatedAt = log.CreatedAt
        }).ToList();
    }

    public async Task CreateAsync(
        Guid projectId,
        Guid userId,
        string action,
        string description)
    {
        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            UserId = userId,
            Action = action,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };

        await _activityLogRepository.CreateAsync(log);
    }
}