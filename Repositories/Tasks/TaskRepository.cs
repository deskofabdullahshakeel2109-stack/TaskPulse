using Microsoft.EntityFrameworkCore;
using TaskPulse.Api.Data;
using TaskPulse.Api.Models;
using TaskPulse.Api.Repositories.Interfaces;

namespace TaskPulse.Api.Repositories.Tasks;

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskItem>> GetAllAsync()
    {
        return await _context.TaskItems
            .AsNoTracking()
            .Include(t => t.Project)
            .Include(t => t.AssignedToUser)
            .Include(t => t.CreatedByUser)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<TaskItem>> GetByProjectIdAsync(Guid projectId)
    {
        return await _context.TaskItems
            .AsNoTracking()
            .Include(t => t.Project)
            .Include(t => t.AssignedToUser)
            .Include(t => t.CreatedByUser)
            .Where(t => t.ProjectId == projectId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<(List<TaskItem> Items, int TotalCount)> GetFilteredAsync(
        Guid projectId,
        string? search,
        string? status,
        string? priority,
        Guid? assignedToUserId,
        string sortBy,
        string sortOrder,
        int page,
        int pageSize)
    {
        IQueryable<TaskItem> query = _context.TaskItems
            .AsNoTracking()
            .Include(t => t.Project)
            .Include(t => t.AssignedToUser)
            .Include(t => t.CreatedByUser)
            .Where(t => t.ProjectId == projectId);

        // Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(t =>
                t.Title.Contains(search) ||
                (t.Description != null &&
                 t.Description.Contains(search)));
        }

        // Status filter
        if (!string.IsNullOrWhiteSpace(status))
        {
            status = status.Trim();

            query = query.Where(t => t.Status == status);
        }

        // Priority filter
        if (!string.IsNullOrWhiteSpace(priority))
        {
            priority = priority.Trim();

            query = query.Where(t => t.Priority == priority);
        }

        // Assigned user filter
        if (assignedToUserId.HasValue)
        {
            query = query.Where(t =>
                t.AssignedToUserId == assignedToUserId.Value);
        }

        // Total count BEFORE pagination
        var totalCount = await query.CountAsync();

        // Sorting
        var descending =
            string.Equals(
                sortOrder,
                "desc",
                StringComparison.OrdinalIgnoreCase);

        sortBy = sortBy?.Trim() ?? "CreatedAt";

        query = sortBy.ToLowerInvariant() switch
        {
            "title" => descending
                ? query.OrderByDescending(t => t.Title)
                : query.OrderBy(t => t.Title),

            "status" => descending
                ? query.OrderByDescending(t => t.Status)
                : query.OrderBy(t => t.Status),

            "priority" => descending
                ? query.OrderByDescending(t => t.Priority)
                : query.OrderBy(t => t.Priority),

            "duedate" => descending
                ? query.OrderByDescending(t => t.DueDate)
                : query.OrderBy(t => t.DueDate),

            "updatedat" => descending
                ? query.OrderByDescending(t => t.UpdatedAt)
                : query.OrderBy(t => t.UpdatedAt),

            "createdat" => descending
                ? query.OrderByDescending(t => t.CreatedAt)
                : query.OrderBy(t => t.CreatedAt),

            _ => query.OrderByDescending(t => t.CreatedAt)
        };

        // Pagination
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await _context.TaskItems
            .Include(t => t.Project)
            .Include(t => t.AssignedToUser)
            .Include(t => t.CreatedByUser)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        await _context.TaskItems.AddAsync(task);

        await _context.SaveChangesAsync();

        return task;
    }

    public async Task<TaskItem?> UpdateAsync(TaskItem task)
    {
        var existingTask = await _context.TaskItems
            .FirstOrDefaultAsync(t => t.Id == task.Id);

        if (existingTask is null)
        {
            return null;
        }

        existingTask.Title = task.Title;
        existingTask.Description = task.Description;
        existingTask.Status = task.Status;
        existingTask.Priority = task.Priority;
        existingTask.AssignedToUserId = task.AssignedToUserId;
        existingTask.DueDate = task.DueDate;
        existingTask.UpdatedAt = task.UpdatedAt;

        await _context.SaveChangesAsync();

        return existingTask;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var task = await _context.TaskItems
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task is null)
        {
            return false;
        }

        _context.TaskItems.Remove(task);

        await _context.SaveChangesAsync();

        return true;
    }
}