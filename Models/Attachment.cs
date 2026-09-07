namespace TaskPulse.Api.Models;

public class Attachment
{
    public Guid Id { get; set; }

    public Guid TaskId { get; set; }

    public Guid UploadedByUserId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string StoredFileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public TaskItem Task { get; set; } = null!;

    public User UploadedByUser { get; set; } = null!;
}