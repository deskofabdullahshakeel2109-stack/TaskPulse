using TaskPulse.Api.Models;

namespace TaskPulse.Api.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<List<Notification>> GetByUserIdAsync(Guid userId);

    Task<List<Notification>> GetUnreadByUserIdAsync(Guid userId);

    Task<Notification?> GetByIdAsync(Guid id);

    Task<Notification> CreateAsync(Notification notification);

    Task<bool> MarkAsReadAsync(Guid id);

    Task<int> MarkAllAsReadAsync(Guid userId);

    Task<bool> DeleteAsync(Guid id);
}