using Microsoft.EntityFrameworkCore;
using TaskPulse.Api.Data;
using TaskPulse.Api.Models;
using TaskPulse.Api.Repositories.Interfaces;

namespace TaskPulse.Api.Repositories.ActivityLogs;

public class ActivityLogRepository : IActivityLogRepository
{
    private readonly ApplicationDbContext _context;

    public ActivityLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ActivityLog> CreateAsync(ActivityLog log)
    {
        await _context.ActivityLogs.AddAsync(log);
        await _context.SaveChangesAsync();

        return log;
    }

    public async Task<List<ActivityLog>> GetByProjectIdAsync(
        Guid projectId)
    {
        return await _context.ActivityLogs
            .AsNoTracking()
            .Include(a => a.User)
            .Where(a => a.ProjectId == projectId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
}