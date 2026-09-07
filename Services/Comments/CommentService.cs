using TaskPulse.Api.DTOs.Comments;
using TaskPulse.Api.Models;
using TaskPulse.Api.Repositories.Interfaces;
using TaskPulse.Api.Services.ActivityLogs;

namespace TaskPulse.Api.Services.Comments;

public class CommentService(
    ICommentRepository repository,
    IActivityLogService activityLogService) : ICommentService
{
    public async Task<List<CommentResponseDto>> GetByTaskIdAsync(
        Guid taskId)
    {
        var comments = await repository.GetByTaskIdAsync(taskId);

        return comments
            .Select(MapToDto)
            .ToList();
    }

    public async Task<CommentResponseDto?> GetByIdAsync(
        Guid id)
    {
        var comment = await repository.GetByIdAsync(id);

        return comment is null
            ? null
            : MapToDto(comment);
    }

    public async Task<CommentResponseDto> CreateAsync(
        Guid taskId,
        CreateCommentDto dto,
        Guid userId)
    {
        if (string.IsNullOrWhiteSpace(dto.Content))
        {
            throw new InvalidOperationException(
                "Comment content is required.");
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            UserId = userId,
            Content = dto.Content.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        var created = await repository.CreateAsync(comment);

        await activityLogService.CreateAsync(
            projectId: await GetProjectIdAsync(taskId),
            userId: userId,
            action: "CommentCreated",
            description: $"Comment created on task {taskId}.");

        var result = await repository.GetByIdAsync(created.Id);

        return MapToDto(result!);
    }

    public async Task<CommentResponseDto?> UpdateAsync(
        Guid id,
        UpdateCommentDto dto,
        Guid userId)
    {
        if (string.IsNullOrWhiteSpace(dto.Content))
        {
            throw new InvalidOperationException(
                "Comment content is required.");
        }

        var existing = await repository.GetByIdAsync(id);

        if (existing is null)
        {
            return null;
        }

        if (existing.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You can only edit your own comments.");
        }

        existing.Content = dto.Content.Trim();
        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await repository.UpdateAsync(existing);

        if (updated is null)
        {
            return null;
        }

        await activityLogService.CreateAsync(
            projectId: await GetProjectIdAsync(existing.TaskId),
            userId: userId,
            action: "CommentUpdated",
            description: $"Comment {id} was updated.");

        var result = await repository.GetByIdAsync(id);

        return MapToDto(result!);
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        Guid userId)
    {
        var existing = await repository.GetByIdAsync(id);

        if (existing is null)
        {
            return false;
        }

        if (existing.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You can only delete your own comments.");
        }

        var projectId = await GetProjectIdAsync(existing.TaskId);

        var deleted = await repository.DeleteAsync(id);

        if (deleted)
        {
            await activityLogService.CreateAsync(
                projectId: projectId,
                userId: userId,
                action: "CommentDeleted",
                description: $"Comment {id} was deleted.");
        }

        return deleted;
    }

    private async Task<Guid> GetProjectIdAsync(Guid taskId)
    {
        var task = await repository.GetTaskByIdAsync(taskId);

        if (task is null)
        {
            throw new InvalidOperationException(
                "Task not found.");
        }

        return task.ProjectId;
    }

    private static CommentResponseDto MapToDto(
        Comment comment)
    {
        return new CommentResponseDto
        {
            Id = comment.Id,
            TaskId = comment.TaskId,
            UserId = comment.UserId,
            UserName = comment.User?.FullName ?? string.Empty,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }
}