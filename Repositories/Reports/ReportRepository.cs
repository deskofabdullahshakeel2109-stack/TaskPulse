using Microsoft.EntityFrameworkCore;
using TaskPulse.Api.Data;
using TaskPulse.Api.Repositories.Interfaces;

namespace TaskPulse.Api.Repositories.Reports;

public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;

    public ReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> GetTotalTasksAsync(Guid projectId)
    {
        return _context.TaskItems
            .AsNoTracking()
            .CountAsync(t => t.ProjectId == projectId);
    }

    public Task<int> GetCompletedTasksAsync(Guid projectId)
    {
        return _context.TaskItems
            .AsNoTracking()
            .CountAsync(t =>
                t.ProjectId == projectId &&
                t.Status == "Completed");
    }

    public Task<int> GetPendingTasksAsync(Guid projectId)
    {
        return _context.TaskItems
            .AsNoTracking()
            .CountAsync(t =>
                t.ProjectId == projectId &&
                t.Status != "Completed");
    }

    public Task<int> GetOverdueTasksAsync(Guid projectId)
    {
        var now = DateTime.UtcNow;

        return _context.TaskItems
            .AsNoTracking()
            .CountAsync(t =>
                t.ProjectId == projectId &&
                t.DueDate.HasValue &&
                t.DueDate.Value < now &&
                t.Status != "Completed");
    }

    public Task<int> GetUnassignedTasksAsync(Guid projectId)
    {
        return _context.TaskItems
            .AsNoTracking()
            .CountAsync(t =>
                t.ProjectId == projectId &&
                t.AssignedToUserId == null);
    }

    public async Task<Dictionary<string, int>>
        GetTasksByStatusAsync(Guid projectId)
    {
        return await _context.TaskItems
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId)
            .GroupBy(t => t.Status)
            .Select(g => new
            {
                Status = g.Key,
                Count = g.Count()
            })
            .ToDictionaryAsync(
                x => x.Status,
                x => x.Count);
    }

    public async Task<Dictionary<string, int>>
        GetTasksByPriorityAsync(Guid projectId)
    {
        return await _context.TaskItems
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId)
            .GroupBy(t => t.Priority)
            .Select(g => new
            {
                Priority = g.Key,
                Count = g.Count()
            })
            .ToDictionaryAsync(
                x => x.Priority,
                x => x.Count);
    }

    public async Task<Dictionary<string, int>>
        GetTasksByAssigneeAsync(Guid projectId)
    {
        return await _context.TaskItems
            .AsNoTracking()
            .Include(t => t.AssignedToUser)
            .Where(t =>
                t.ProjectId == projectId &&
                t.AssignedToUserId != null)
            .GroupBy(t =>
                t.AssignedToUser!.FullName)
            .Select(g => new
            {
                UserName = g.Key,
                Count = g.Count()
            })
            .ToDictionaryAsync(
                x => x.UserName,
                x => x.Count);
    }
}