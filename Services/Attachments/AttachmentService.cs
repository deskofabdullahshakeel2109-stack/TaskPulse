using TaskPulse.Api.DTOs.Attachments;
using TaskPulse.Api.Models;
using TaskPulse.Api.Repositories.Interfaces;
using TaskPulse.Api.Services.ActivityLogs;

namespace TaskPulse.Api.Services.Attachments;

public class AttachmentService(
    IAttachmentRepository repository,
    IActivityLogService activityLogService,
    ITaskRepository taskRepository,
    IWebHostEnvironment environment) : IAttachmentService
{
    private const long MaxFileSize = 10 * 1024 * 1024;

    private static readonly string[] AllowedExtensions =
    [
        ".pdf",
        ".doc",
        ".docx",
        ".xls",
        ".xlsx",
        ".txt",
        ".png",
        ".jpg",
        ".jpeg"
    ];

    public async Task<List<AttachmentResponseDto>> GetByTaskIdAsync(
        Guid taskId)
    {
        var attachments =
            await repository.GetByTaskIdAsync(taskId);

        return attachments
            .Select(MapToDto)
            .ToList();
    }

    public async Task<AttachmentResponseDto?> GetByIdAsync(
        Guid id)
    {
        var attachment =
            await repository.GetByIdAsync(id);

        return attachment is null
            ? null
            : MapToDto(attachment);
    }

    public async Task<AttachmentResponseDto> UploadAsync(
        Guid taskId,
        Guid userId,
        IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            throw new InvalidOperationException(
                "A file is required.");
        }

        if (file.Length > MaxFileSize)
        {
            throw new InvalidOperationException(
                "File size cannot exceed 10 MB.");
        }

        var extension =
            Path.GetExtension(file.FileName)
                .ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException(
                "This file type is not allowed.");
        }

        var task =
            await taskRepository.GetByIdAsync(taskId);

        if (task is null)
        {
            throw new InvalidOperationException(
                "Task not found.");
        }

        var uploadsFolder = Path.Combine(
            environment.ContentRootPath,
            "Uploads",
            "Attachments");

        Directory.CreateDirectory(uploadsFolder);

        var storedFileName =
            $"{Guid.NewGuid()}{extension}";

        var filePath = Path.Combine(
            uploadsFolder,
            storedFileName);

        try
        {
            await using (var stream = new FileStream(
                filePath,
                FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                UploadedByUserId = userId,
                FileName = Path.GetFileName(file.FileName),
                StoredFileName = storedFileName,
                ContentType = string.IsNullOrWhiteSpace(
                    file.ContentType)
                    ? "application/octet-stream"
                    : file.ContentType,
                FileSize = file.Length,
                CreatedAt = DateTime.UtcNow
            };

            var created =
                await repository.CreateAsync(attachment);

            await activityLogService.CreateAsync(
                projectId: task.ProjectId,
                userId: userId,
                action: "AttachmentUploaded",
                description:
                    $"Attachment '{attachment.FileName}' was uploaded.");

            var result =
                await repository.GetByIdAsync(created.Id);

            return MapToDto(result!);
        }
        catch
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            throw;
        }
    }

    public async Task<(Stream Stream, string ContentType, string FileName)?>
        GetFileAsync(Guid id)
    {
        var attachment =
            await repository.GetByIdAsync(id);

        if (attachment is null)
        {
            return null;
        }

        var uploadsFolder = Path.Combine(
            environment.ContentRootPath,
            "Uploads",
            "Attachments");

        var filePath = Path.Combine(
            uploadsFolder,
            attachment.StoredFileName);

        if (!File.Exists(filePath))
        {
            return null;
        }

        var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read);

        return (
            stream,
            string.IsNullOrWhiteSpace(attachment.ContentType)
                ? "application/octet-stream"
                : attachment.ContentType,
            attachment.FileName
        );
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var attachment =
            await repository.GetByIdAsync(id);

        if (attachment is null)
        {
            return false;
        }

        var uploadsFolder = Path.Combine(
            environment.ContentRootPath,
            "Uploads",
            "Attachments");

        var filePath = Path.Combine(
            uploadsFolder,
            attachment.StoredFileName);

        var deleted =
            await repository.DeleteAsync(id);

        if (deleted && File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return deleted;
    }

    private static AttachmentResponseDto MapToDto(
        Attachment attachment)
    {
        return new AttachmentResponseDto
        {
            Id = attachment.Id,
            TaskId = attachment.TaskId,
            UploadedByUserId = attachment.UploadedByUserId,
            FileName = attachment.FileName,
            ContentType = attachment.ContentType,
            FileSize = attachment.FileSize,
            CreatedAt = attachment.CreatedAt,
            UploadedByUserName =
                attachment.UploadedByUser?.FullName
                ?? string.Empty
        };
    }
}