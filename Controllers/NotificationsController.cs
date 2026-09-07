using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskPulse.Api.DTOs.Notifications;
using TaskPulse.Api.Services.CurrentUser;
using TaskPulse.Api.Services.Notifications;

namespace TaskPulse.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController(
    INotificationService notificationService,
    ICurrentUserService currentUser) : ControllerBase
{
    // =========================================================
    // GET ALL MY NOTIFICATIONS
    // =========================================================

    [HttpGet]
    public async Task<ActionResult<List<NotificationResponseDto>>>
        GetMyNotifications()
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        return Ok(
            await notificationService
                .GetMyNotificationsAsync(userId));
    }

    // =========================================================
    // GET UNREAD NOTIFICATIONS
    // =========================================================

    [HttpGet("unread")]
    public async Task<ActionResult<List<NotificationResponseDto>>>
        GetMyUnreadNotifications()
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        return Ok(
            await notificationService
                .GetUnreadAsync(userId));
    }

    // =========================================================
    // GET UNREAD COUNT
    // =========================================================

    [HttpGet("unread/count")]
    public async Task<ActionResult<int>>
        GetUnreadCount()
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var count =
            await notificationService
                .GetUnreadCountAsync(userId);

        return Ok(new
        {
            unreadCount = count
        });
    }

    // =========================================================
    // GET NOTIFICATION BY ID
    // =========================================================

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<NotificationResponseDto>>
        GetById(Guid id)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var notification =
            await notificationService.GetByIdAsync(
                id,
                userId);

        if (notification is null)
        {
            return NotFound(new
            {
                message = "Notification not found."
            });
        }

        return Ok(notification);
    }

    // =========================================================
    // MARK ONE AS READ
    // =========================================================

    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult>
        MarkAsRead(Guid id)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var notification =
            await notificationService.GetByIdAsync(
                id,
                userId);

        if (notification is null)
        {
            return NotFound(new
            {
                message = "Notification not found."
            });
        }

        var updated =
            await notificationService.MarkAsReadAsync(
                id,
                userId);

        return updated
            ? Ok(new
            {
                message = "Notification marked as read."
            })
            : NotFound(new
            {
                message = "Notification not found."
            });
    }

    // =========================================================
    // MARK ALL AS READ
    // =========================================================

    [HttpPut("read-all")]
    public async Task<IActionResult>
        MarkAllAsRead()
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var count =
            await notificationService
                .MarkAllAsReadAsync(userId);

        return Ok(new
        {
            message = "All notifications marked as read.",
            updatedCount = count
        });
    }

    // =========================================================
    // DELETE NOTIFICATION
    // =========================================================

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult>
        Delete(Guid id)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var deleted =
            await notificationService.DeleteAsync(
                id,
                userId);

        return deleted
            ? Ok(new
            {
                message = "Notification deleted successfully."
            })
            : NotFound(new
            {
                message = "Notification not found."
            });
    }
}