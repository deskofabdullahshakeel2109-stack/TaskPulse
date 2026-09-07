using TaskPulse.Api.Models;

namespace TaskPulse.Api.Repositories.Interfaces;

public interface IAttachmentRepository
{
    Task<List<Attachment>> GetByTaskIdAsync(Guid taskId);

    Task<Attachment?> GetByIdAsync(Guid id);

    Task<Attachment> CreateAsync(Attachment attachment);

    Task<bool> DeleteAsync(Guid id);
}