using TaskPulse.Api.DTOs.Tasks;
using TaskPulse.Api.Models;
using TaskPulse.Api.Repositories.Interfaces;
using TaskPulse.Api.Services.ActivityLogs;
using TaskPulse.Api.Services.Notifications;
using TaskPulse.Api.Services.ProjectMembers;

namespace TaskPulse.Api.Services.Tasks;

public class TaskService(
    ITaskRepository repository,
    IProjectMemberService projectMemberService,
    IActivityLogService activityLogService,
    INotificationService notificationService) : ITaskService
{
    private static readonly string[] AllowedStatuses =
    [
        "Todo",
        "InProgress",
        "Completed"
    ];

    private static readonly string[] AllowedPriorities =
    [
        "Low",
        "Medium",
        "High"
    ];

    // =========================================================
    // GET ALL TASKS
    // =========================================================

    public async Task<List<TaskResponseDto>> GetAllAsync()
    {
        var tasks = await repository.GetAllAsync();

        return tasks
            .Select(MapToDto)
            .ToList();
    }

    // =========================================================
    // GET TASKS BY PROJECT
    // =========================================================

    public async Task<List<TaskResponseDto>> GetByProjectIdAsync(
        Guid projectId)
    {
        var tasks = await repository.GetByProjectIdAsync(projectId);

        return tasks
            .Select(MapToDto)
            .ToList();
    }

    // =========================================================
    // FILTER / SEARCH / SORT / PAGINATION
    // =========================================================

    public async Task<List<TaskResponseDto>> GetFilteredAsync(
        Guid projectId,
        TaskQueryDto query)
    {
        var tasks = await repository.GetByProjectIdAsync(projectId);

        // -----------------------------------------------------
        // SEARCH
        // -----------------------------------------------------

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            tasks = tasks
                .Where(t =>
                    t.Title.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    (
                        t.Description != null &&
                        t.Description.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase)
                    ))
                .ToList();
        }

        // -----------------------------------------------------
        // STATUS FILTER
        // -----------------------------------------------------

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            var status = query.Status.Trim();

            tasks = tasks
                .Where(t =>
                    t.Status.Equals(
                        status,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // -----------------------------------------------------
        // PRIORITY FILTER
        // -----------------------------------------------------

        if (!string.IsNullOrWhiteSpace(query.Priority))
        {
            var priority = query.Priority.Trim();

            tasks = tasks
                .Where(t =>
                    t.Priority.Equals(
                        priority,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // -----------------------------------------------------
        // ASSIGNED USER FILTER
        // -----------------------------------------------------

        if (query.AssignedToUserId.HasValue)
        {
            tasks = tasks
                .Where(t =>
                    t.AssignedToUserId ==
                    query.AssignedToUserId.Value)
                .ToList();
        }

        // -----------------------------------------------------
        // SORTING
        // -----------------------------------------------------

        var sortBy =
            query.SortBy?
                .Trim()
                .ToLowerInvariant();

        var sortOrder =
            query.SortOrder?
                .Trim()
                .ToLowerInvariant();

        var descending = sortOrder == "desc";

        tasks = sortBy switch
        {
            "title" =>
                descending
                    ? tasks.OrderByDescending(t => t.Title).ToList()
                    : tasks.OrderBy(t => t.Title).ToList(),

            "status" =>
                descending
                    ? tasks.OrderByDescending(t => t.Status).ToList()
                    : tasks.OrderBy(t => t.Status).ToList(),

            "priority" =>
                descending
                    ? tasks.OrderByDescending(t => t.Priority).ToList()
                    : tasks.OrderBy(t => t.Priority).ToList(),

            "duedate" =>
                descending
                    ? tasks.OrderByDescending(t => t.DueDate).ToList()
                    : tasks.OrderBy(t => t.DueDate).ToList(),

            "updatedat" =>
                descending
                    ? tasks.OrderByDescending(t => t.UpdatedAt).ToList()
                    : tasks.OrderBy(t => t.UpdatedAt).ToList(),

            _ =>
                descending
                    ? tasks.OrderByDescending(t => t.CreatedAt).ToList()
                    : tasks.OrderBy(t => t.CreatedAt).ToList()
        };

        // -----------------------------------------------------
        // PAGINATION VALIDATION
        // -----------------------------------------------------

        var page = query.Page < 1
            ? 1
            : query.Page;

        var pageSize = query.PageSize < 1
            ? 10
            : query.PageSize;

        if (pageSize > 100)
        {
            pageSize = 100;
        }

        // -----------------------------------------------------
        // PAGINATION
        // -----------------------------------------------------

        tasks = tasks
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // -----------------------------------------------------
        // MAP TO DTO
        // -----------------------------------------------------

        return tasks
            .Select(MapToDto)
            .ToList();
    }

    // =========================================================
    // GET TASK BY ID
    // =========================================================

    public async Task<TaskResponseDto?> GetByIdAsync(Guid id)
    {
        var task = await repository.GetByIdAsync(id);

        return task is null
            ? null
            : MapToDto(task);
    }

    // =========================================================
    // CREATE TASK
    // =========================================================

    public async Task<TaskResponseDto> CreateAsync(
        Guid projectId,
        CreateTaskDto dto,
        Guid createdByUserId)
    {
        // Validate status
        ValidateStatus(dto.Status);

        // Validate priority
        ValidatePriority(dto.Priority);

        // Validate assignment
        await ValidateAssignmentAsync(
            projectId,
            dto.AssignedToUserId);

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            AssignedToUserId = dto.AssignedToUserId,
            CreatedByUserId = createdByUserId,
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            Status = dto.Status.Trim(),
            Priority = dto.Priority.Trim(),
            DueDate = dto.DueDate,
            CreatedAt = DateTime.UtcNow
        };

        var created = await repository.CreateAsync(task);

        // -----------------------------------------------------
        // ACTIVITY LOG
        // -----------------------------------------------------

        await activityLogService.CreateAsync(
            projectId,
            createdByUserId,
            "TaskCreated",
            $"Task '{created.Title}' was created.");

        // -----------------------------------------------------
        // NOTIFICATION - TASK ASSIGNED
        // -----------------------------------------------------

        if (created.AssignedToUserId.HasValue)
        {
            await notificationService.CreateAsync(
                created.AssignedToUserId.Value,
                created.ProjectId,
                created.Id,
                "TaskAssigned",
                $"You have been assigned a new task: {created.Title}");
        }

        var result =
            await repository.GetByIdAsync(created.Id);

        return MapToDto(result!);
    }

    // =========================================================
    // UPDATE TASK
    // =========================================================

    public async Task<TaskResponseDto?> UpdateAsync(
        Guid id,
        UpdateTaskDto dto,
        Guid updatedByUserId)
    {
        var existing =
            await repository.GetByIdAsync(id);

        if (existing is null)
        {
            return null;
        }

        // -----------------------------------------------------
        // STORE OLD VALUES
        // -----------------------------------------------------

        var oldStatus =
            existing.Status;

        var oldPriority =
            existing.Priority;

        var oldAssignedToUserId =
            existing.AssignedToUserId;

        var oldDueDate =
            existing.DueDate;

        // -----------------------------------------------------
        // VALIDATION
        // -----------------------------------------------------

        ValidateStatus(dto.Status);

        ValidatePriority(dto.Priority);

        await ValidateAssignmentAsync(
            existing.ProjectId,
            dto.AssignedToUserId);

        // -----------------------------------------------------
        // UPDATE FIELDS
        // -----------------------------------------------------

        existing.Title =
            dto.Title.Trim();

        existing.Description =
            dto.Description?.Trim();

        existing.Status =
            dto.Status.Trim();

        existing.Priority =
            dto.Priority.Trim();

        existing.AssignedToUserId =
            dto.AssignedToUserId;

        existing.DueDate =
            dto.DueDate;

        existing.UpdatedAt =
            DateTime.UtcNow;

        var updated =
            await repository.UpdateAsync(existing);

        if (updated is null)
        {
            return null;
        }

        // =====================================================
        // ACTIVITY LOG - GENERAL UPDATE
        // =====================================================

        await activityLogService.CreateAsync(
            existing.ProjectId,
            updatedByUserId,
            "TaskUpdated",
            $"Task '{existing.Title}' was updated.");

        // =====================================================
        // ACTIVITY LOG - STATUS CHANGE
        // =====================================================

        if (!string.Equals(
            oldStatus,
            existing.Status,
            StringComparison.OrdinalIgnoreCase))
        {
            await activityLogService.CreateAsync(
                existing.ProjectId,
                updatedByUserId,
                "TaskStatusChanged",
                $"Task '{existing.Title}' status changed from '{oldStatus}' to '{existing.Status}'.");
        }

        // =====================================================
        // NOTIFICATION - TASK COMPLETED
        // =====================================================

        if (!string.Equals(
                oldStatus,
                "Completed",
                StringComparison.OrdinalIgnoreCase)
            &&
            string.Equals(
                existing.Status,
                "Completed",
                StringComparison.OrdinalIgnoreCase))
        {
            if (existing.AssignedToUserId.HasValue)
            {
                await notificationService.CreateAsync(
                    existing.AssignedToUserId.Value,
                    existing.ProjectId,
                    existing.Id,
                    "TaskCompleted",
                    $"Task '{existing.Title}' has been completed.");
            }
        }

        // =====================================================
        // ACTIVITY LOG - PRIORITY CHANGE
        // =====================================================

        if (!string.Equals(
            oldPriority,
            existing.Priority,
            StringComparison.OrdinalIgnoreCase))
        {
            await activityLogService.CreateAsync(
                existing.ProjectId,
                updatedByUserId,
                "TaskPriorityChanged",
                $"Task '{existing.Title}' priority changed from '{oldPriority}' to '{existing.Priority}'.");
        }

        // =====================================================
        // ACTIVITY LOG + NOTIFICATION - ASSIGNMENT CHANGE
        // =====================================================

        if (oldAssignedToUserId !=
            existing.AssignedToUserId)
        {
            var description =
                existing.AssignedToUserId is null
                    ? $"Task '{existing.Title}' was unassigned."
                    : $"Task '{existing.Title}' was assigned to user '{existing.AssignedToUserId}'.";

            await activityLogService.CreateAsync(
                existing.ProjectId,
                updatedByUserId,
                "TaskAssigned",
                description);

            // Notify the NEW assignee
            if (existing.AssignedToUserId.HasValue)
            {
                await notificationService.CreateAsync(
                    existing.AssignedToUserId.Value,
                    existing.ProjectId,
                    existing.Id,
                    "TaskAssigned",
                    $"You have been assigned a task: {existing.Title}");
            }
        }

        // =====================================================
        // ACTIVITY LOG - DUE DATE CHANGE
        // =====================================================

        if (oldDueDate !=
            existing.DueDate)
        {
            var description =
                existing.DueDate is null
                    ? $"Task '{existing.Title}' due date was removed."
                    : $"Task '{existing.Title}' due date changed to '{existing.DueDate:yyyy-MM-dd HH:mm}'.";

            await activityLogService.CreateAsync(
                existing.ProjectId,
                updatedByUserId,
                "TaskDueDateChanged",
                description);
        }

        // =====================================================
        // RETURN UPDATED TASK
        // =====================================================

        var result =
            await repository.GetByIdAsync(id);

        return MapToDto(result!);
    }

    // =========================================================
    // DELETE TASK
    // =========================================================

    public async Task<bool> DeleteAsync(
        Guid id,
        Guid deletedByUserId)
    {
        var existing =
            await repository.GetByIdAsync(id);

        if (existing is null)
        {
            return false;
        }

        var projectId =
            existing.ProjectId;

        var title =
            existing.Title;

        var assignedToUserId =
            existing.AssignedToUserId;

        var deleted =
            await repository.DeleteAsync(id);

        if (!deleted)
        {
            return false;
        }

        // Activity log
        await activityLogService.CreateAsync(
            projectId,
            deletedByUserId,
            "TaskDeleted",
            $"Task '{title}' was deleted.");

        // Notification to assigned user
        if (assignedToUserId.HasValue)
        {
            await notificationService.CreateAsync(
                assignedToUserId.Value,
                projectId,
                null,
                "TaskDeleted",
                $"The task '{title}' assigned to you was deleted.");
        }

        return true;
    }

    // =========================================================
    // VALIDATE ASSIGNMENT
    // =========================================================

    private async Task ValidateAssignmentAsync(
        Guid projectId,
        Guid? assignedToUserId)
    {
        // Null = unassigned
        if (assignedToUserId is null)
        {
            return;
        }

        var members =
            await projectMemberService
                .GetByProjectIdAsync(projectId);

        var isMember =
            members.Any(
                member =>
                    member.UserId ==
                    assignedToUserId.Value);

        if (!isMember)
        {
            throw new InvalidOperationException(
                "The assigned user must be a member of this project.");
        }
    }

    // =========================================================
    // VALIDATE STATUS
    // =========================================================

    private static void ValidateStatus(
        string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            throw new InvalidOperationException(
                "Task status is required.");
        }

        var valid =
            AllowedStatuses.Any(
                value =>
                    value.Equals(
                        status.Trim(),
                        StringComparison.OrdinalIgnoreCase));

        if (!valid)
        {
            throw new InvalidOperationException(
                "Invalid task status. Allowed values: Todo, InProgress, Completed.");
        }
    }

    // =========================================================
    // VALIDATE PRIORITY
    // =========================================================

    private static void ValidatePriority(
        string? priority)
    {
        if (string.IsNullOrWhiteSpace(priority))
        {
            throw new InvalidOperationException(
                "Task priority is required.");
        }

        var valid =
            AllowedPriorities.Any(
                value =>
                    value.Equals(
                        priority.Trim(),
                        StringComparison.OrdinalIgnoreCase));

        if (!valid)
        {
            throw new InvalidOperationException(
                "Invalid task priority. Allowed values: Low, Medium, High.");
        }
    }

    // =========================================================
    // MAP ENTITY -> DTO
    // =========================================================

    private static TaskResponseDto MapToDto(
        TaskItem task)
    {
        return new TaskResponseDto
        {
            Id =
                task.Id,

            ProjectId =
                task.ProjectId,

            AssignedToUserId =
                task.AssignedToUserId,

            CreatedByUserId =
                task.CreatedByUserId,

            Title =
                task.Title,

            Description =
                task.Description,

            Status =
                task.Status,

            Priority =
                task.Priority,

            DueDate =
                task.DueDate,

            CreatedAt =
                task.CreatedAt,

            UpdatedAt =
                task.UpdatedAt,

            AssignedToUserName =
                task.AssignedToUser?.FullName,

            CreatedByUserName =
                task.CreatedByUser?.FullName
        };
    }
}