using TaskPulse.Api.DTOs.Notifications;
using TaskPulse.Api.Models;
using TaskPulse.Api.Repositories.Interfaces;

namespace TaskPulse.Api.Services.Notifications;

public class NotificationService(
    INotificationRepository repository) : INotificationService
{
    // =========================================================
    // GET ALL MY NOTIFICATIONS
    // =========================================================

    public async Task<List<NotificationResponseDto>>
        GetMyNotificationsAsync(Guid userId)
    {
        var notifications =
            await repository.GetByUserIdAsync(userId);

        return notifications
            .Select(MapToDto)
            .ToList();
    }

    // =========================================================
    // GET MY UNREAD NOTIFICATIONS
    // =========================================================

    public async Task<List<NotificationResponseDto>>
        GetMyUnreadNotificationsAsync(Guid userId)
    {
        var notifications =
            await repository.GetUnreadByUserIdAsync(userId);

        return notifications
            .Select(MapToDto)
            .ToList();
    }

    // =========================================================
    // GET UNREAD NOTIFICATIONS
    // =========================================================

    public async Task<List<NotificationResponseDto>>
        GetUnreadAsync(Guid userId)
    {
        return await GetMyUnreadNotificationsAsync(userId);
    }

    // =========================================================
    // GET UNREAD COUNT
    // =========================================================

    public async Task<int>
        GetUnreadCountAsync(Guid userId)
    {
        var notifications =
            await repository.GetUnreadByUserIdAsync(userId);

        return notifications.Count;
    }

    // =========================================================
    // GET NOTIFICATION BY ID
    // =========================================================

    public async Task<NotificationResponseDto?>
        GetByIdAsync(
            Guid id,
            Guid userId)
    {
        var notification =
            await repository.GetByIdAsync(id);

        if (notification is null)
        {
            return null;
        }

        // Users can only access their own notifications.
        if (notification.UserId != userId)
        {
            return null;
        }

        return MapToDto(notification);
    }

    // =========================================================
    // CREATE NOTIFICATION
    // =========================================================

    public async Task CreateAsync(
        Guid userId,
        Guid? projectId,
        Guid? taskId,
        string type,
        string message)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new InvalidOperationException(
                "Notification type is required.");
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new InvalidOperationException(
                "Notification message is required.");
        }

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProjectId = projectId,
            TaskId = taskId,
            Type = type.Trim(),
            Message = message.Trim(),
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await repository.CreateAsync(notification);
    }

    // =========================================================
    // MARK AS READ
    // =========================================================

    public async Task<bool>
        MarkAsReadAsync(
            Guid notificationId,
            Guid userId)
    {
        var notification =
            await repository.GetByIdAsync(notificationId);

        if (notification is null)
        {
            return false;
        }

        // Users can only modify their own notifications.
        if (notification.UserId != userId)
        {
            return false;
        }

        return await repository.MarkAsReadAsync(
            notificationId);
    }

    // =========================================================
    // MARK ALL AS READ
    // =========================================================

    public async Task<int>
        MarkAllAsReadAsync(Guid userId)
    {
        return await repository.MarkAllAsReadAsync(userId);
    }

    // =========================================================
    // DELETE NOTIFICATION
    // =========================================================

    public async Task<bool>
        DeleteAsync(
            Guid id,
            Guid userId)
    {
        var notification =
            await repository.GetByIdAsync(id);

        if (notification is null)
        {
            return false;
        }

        // Users can only delete their own notifications.
        if (notification.UserId != userId)
        {
            return false;
        }

        return await repository.DeleteAsync(id);
    }

    // =========================================================
    // MAP ENTITY TO DTO
    // =========================================================

    private static NotificationResponseDto MapToDto(
        Notification notification)
    {
        return new NotificationResponseDto
        {
            Id = notification.Id,
            UserId = notification.UserId,
            ProjectId = notification.ProjectId,
            TaskId = notification.TaskId,
            Type = notification.Type,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        };
    }
}