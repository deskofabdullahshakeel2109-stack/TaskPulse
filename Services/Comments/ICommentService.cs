using TaskPulse.Api.DTOs.Comments;

namespace TaskPulse.Api.Services.Comments;

public interface ICommentService
{
    Task<List<CommentResponseDto>> GetByTaskIdAsync(Guid taskId);

    Task<CommentResponseDto?> GetByIdAsync(Guid id);

    Task<CommentResponseDto> CreateAsync(
        Guid taskId,
        CreateCommentDto dto,
        Guid userId);

    Task<CommentResponseDto?> UpdateAsync(
        Guid id,
        UpdateCommentDto dto,
        Guid userId);

    Task<bool> DeleteAsync(
        Guid id,
        Guid userId);
}