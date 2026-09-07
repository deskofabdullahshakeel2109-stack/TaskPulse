namespace TaskPulse.Api.DTOs.Attachments;

public class AttachmentResponseDto
{
    public Guid Id { get; set; }

    public Guid TaskId { get; set; }

    public Guid UploadedByUserId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public DateTime CreatedAt { get; set; }

    public string UploadedByUserName { get; set; } = string.Empty;
}