using TaskPulse.Api.DTOs.Attachments;

namespace TaskPulse.Api.Services.Attachments;

public interface IAttachmentService
{
    Task<List<AttachmentResponseDto>> GetByTaskIdAsync(
        Guid taskId);

    Task<AttachmentResponseDto?> GetByIdAsync(
        Guid id);

    Task<AttachmentResponseDto> UploadAsync(
        Guid taskId,
        Guid userId,
        IFormFile file);

    Task<(Stream Stream, string ContentType, string FileName)?> GetFileAsync(
        Guid id);

    Task<bool> DeleteAsync(
        Guid id);
}