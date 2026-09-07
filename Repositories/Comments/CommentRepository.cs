using Microsoft.EntityFrameworkCore;
using TaskPulse.Api.Data;
using TaskPulse.Api.Models;
using TaskPulse.Api.Repositories.Interfaces;

namespace TaskPulse.Api.Repositories.Comments;

public class CommentRepository : ICommentRepository
{
    private readonly ApplicationDbContext _context;

    public CommentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Comment>> GetByTaskIdAsync(Guid taskId)
    {
        return _context.Comments
            .AsNoTracking()
            .Include(c => c.User)
            .Where(c => c.TaskId == taskId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public Task<Comment?> GetByIdAsync(Guid id)
    {
        return _context.Comments
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Comment> CreateAsync(Comment comment)
    {
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        return comment;
    }

    public async Task<Comment?> UpdateAsync(Comment comment)
    {
        var existing = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == comment.Id);

        if (existing is null)
        {
            return null;
        }

        existing.Content = comment.Content;
        existing.UpdatedAt = comment.UpdatedAt;

        await _context.SaveChangesAsync();

        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment is null)
        {
            return false;
        }

        _context.Comments.Remove(comment);

        await _context.SaveChangesAsync();

        return true;
    }
    public Task<TaskItem?> GetTaskByIdAsync(Guid taskId)
    {
        return _context.TaskItems
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == taskId);
    }
}