using TaskPulse.Api.DTOs.Notifications;

namespace TaskPulse.Api.Services.Notifications;

public interface INotificationService
{
    Task<List<NotificationResponseDto>>
        GetMyNotificationsAsync(Guid userId);

    Task<List<NotificationResponseDto>>
        GetMyUnreadNotificationsAsync(Guid userId);

    Task<List<NotificationResponseDto>>
        GetUnreadAsync(Guid userId);

    Task<int>
        GetUnreadCountAsync(Guid userId);

    Task<NotificationResponseDto?>
        GetByIdAsync(Guid id, Guid userId);

    Task CreateAsync(
        Guid userId,
        Guid? projectId,
        Guid? taskId,
        string type,
        string message);

    Task<bool>
        MarkAsReadAsync(
            Guid notificationId,
            Guid userId);

    Task<int>
        MarkAllAsReadAsync(Guid userId);

    Task<bool>
        DeleteAsync(
            Guid id,
            Guid userId);
}