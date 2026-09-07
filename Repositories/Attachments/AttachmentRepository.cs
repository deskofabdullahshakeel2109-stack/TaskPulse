using Microsoft.EntityFrameworkCore;
using TaskPulse.Api.Data;
using TaskPulse.Api.Models;
using TaskPulse.Api.Repositories.Interfaces;

namespace TaskPulse.Api.Repositories.Attachments;

public class AttachmentRepository : IAttachmentRepository
{
    private readonly ApplicationDbContext _context;

    public AttachmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Attachment>> GetByTaskIdAsync(Guid taskId)
    {
        return _context.Attachments
            .AsNoTracking()
            .Include(a => a.UploadedByUser)
            .Where(a => a.TaskId == taskId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public Task<Attachment?> GetByIdAsync(Guid id)
    {
        return _context.Attachments
            .Include(a => a.UploadedByUser)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Attachment> CreateAsync(
        Attachment attachment)
    {
        await _context.Attachments.AddAsync(attachment);

        await _context.SaveChangesAsync();

        return attachment;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var attachment = await _context.Attachments
            .FirstOrDefaultAsync(a => a.Id == id);

        if (attachment is null)
        {
            return false;
        }

        _context.Attachments.Remove(attachment);

        await _context.SaveChangesAsync();

        return true;
    }
}