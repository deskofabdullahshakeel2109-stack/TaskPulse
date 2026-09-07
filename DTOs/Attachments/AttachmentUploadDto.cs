using Microsoft.AspNetCore.Http;

namespace TaskPulse.Api.DTOs.Attachments;

public class AttachmentUploadDto
{
    public IFormFile File { get; set; } = null!;
}