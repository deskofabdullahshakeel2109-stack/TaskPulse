using TaskPulse.Api.Models;

namespace TaskPulse.Api.Repositories.Interfaces;

public interface ICommentRepository
{
    Task<List<Comment>> GetByTaskIdAsync(Guid taskId);

    Task<Comment?> GetByIdAsync(Guid id);

    Task<Comment> CreateAsync(Comment comment);

    Task<Comment?> UpdateAsync(Comment comment);

    Task<bool> DeleteAsync(Guid id);

    Task<TaskItem?> GetTaskByIdAsync(Guid taskId);
}