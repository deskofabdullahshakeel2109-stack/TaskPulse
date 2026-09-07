using Microsoft.EntityFrameworkCore;
using TaskPulse.Api.Data;
using TaskPulse.Api.Models;
using TaskPulse.Api.Repositories.Interfaces;

namespace TaskPulse.Api.Repositories.Notifications;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Notification>> GetByUserIdAsync(Guid userId)
    {
        return _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public Task<List<Notification>> GetUnreadByUserIdAsync(Guid userId)
    {
        return _context.Notifications
            .AsNoTracking()
            .Where(n =>
                n.UserId == userId &&
                !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public Task<Notification?> GetByIdAsync(Guid id)
    {
        return _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<Notification> CreateAsync(
        Notification notification)
    {
        await _context.Notifications.AddAsync(notification);

        await _context.SaveChangesAsync();

        return notification;
    }

    public async Task<bool> MarkAsReadAsync(Guid id)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id);

        if (notification is null)
        {
            return false;
        }

        notification.IsRead = true;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<int> MarkAllAsReadAsync(Guid userId)
    {
        var notifications = await _context.Notifications
            .Where(n =>
                n.UserId == userId &&
                !n.IsRead)
            .ToListAsync();

        if (notifications.Count == 0)
        {
            return 0;
        }

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync();

        return notifications.Count;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id);

        if (notification is null)
        {
            return false;
        }

        _context.Notifications.Remove(notification);

        await _context.SaveChangesAsync();

        return true;
    }
}