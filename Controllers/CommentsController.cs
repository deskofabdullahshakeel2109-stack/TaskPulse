using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskPulse.Api.DTOs.Comments;
using TaskPulse.Api.Services.Comments;
using TaskPulse.Api.Services.CurrentUser;
using TaskPulse.Api.Services.ProjectMembers;
using TaskPulse.Api.Services.Projects;
using TaskPulse.Api.Services.Tasks;

namespace TaskPulse.Api.Controllers;

[ApiController]
[Route("api/tasks/{taskId:guid}/comments")]
[Authorize]
public class CommentsController(
    ICommentService commentService,
    ITaskService taskService,
    IProjectService projectService,
    IProjectMemberService memberService,
    ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CommentResponseDto>>> GetByTask(
        Guid taskId)
    {
        var task = await taskService.GetByIdAsync(taskId);

        if (task is null)
        {
            return NotFound(new
            {
                message = "Task not found."
            });
        }

        if (!await CanAccessProject(task.ProjectId))
        {
            return Forbid();
        }

        return Ok(
            await commentService.GetByTaskIdAsync(taskId));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CommentResponseDto>> GetById(
        Guid taskId,
        Guid id)
    {
        var task = await taskService.GetByIdAsync(taskId);

        if (task is null)
        {
            return NotFound(new
            {
                message = "Task not found."
            });
        }

        if (!await CanAccessProject(task.ProjectId))
        {
            return Forbid();
        }

        var comment = await commentService.GetByIdAsync(id);

        if (comment is null || comment.TaskId != taskId)
        {
            return NotFound(new
            {
                message = "Comment not found."
            });
        }

        return Ok(comment);
    }

    [HttpPost]
    public async Task<ActionResult<CommentResponseDto>> Create(
        Guid taskId,
        CreateCommentDto dto)
    {
        var task = await taskService.GetByIdAsync(taskId);

        if (task is null)
        {
            return NotFound(new
            {
                message = "Task not found."
            });
        }

        if (!await CanAccessProject(task.ProjectId))
        {
            return Forbid();
        }

        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        try
        {
            var comment = await commentService.CreateAsync(
                taskId,
                dto,
                userId);

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    taskId,
                    id = comment.Id
                },
                comment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CommentResponseDto>> Update(
        Guid taskId,
        Guid id,
        UpdateCommentDto dto)
    {
        var task = await taskService.GetByIdAsync(taskId);

        if (task is null)
        {
            return NotFound(new
            {
                message = "Task not found."
            });
        }

        if (!await CanAccessProject(task.ProjectId))
        {
            return Forbid();
        }

        var existing = await commentService.GetByIdAsync(id);

        if (existing is null || existing.TaskId != taskId)
        {
            return NotFound(new
            {
                message = "Comment not found."
            });
        }

        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        try
        {
            var updated = await commentService.UpdateAsync(
                id,
                dto,
                userId);

            return updated is null
                ? NotFound(new
                {
                    message = "Comment not found."
                })
                : Ok(updated);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid taskId,
        Guid id)
    {
        var task = await taskService.GetByIdAsync(taskId);

        if (task is null)
        {
            return NotFound(new
            {
                message = "Task not found."
            });
        }

        if (!await CanAccessProject(task.ProjectId))
        {
            return Forbid();
        }

        var existing = await commentService.GetByIdAsync(id);

        if (existing is null || existing.TaskId != taskId)
        {
            return NotFound(new
            {
                message = "Comment not found."
            });
        }

        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        try
        {
            var deleted = await commentService.DeleteAsync(
                id,
                userId);

            return deleted
                ? Ok(new
                {
                    message = "Comment deleted successfully."
                })
                : NotFound(new
                {
                    message = "Comment not found."
                });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    private async Task<bool> CanAccessProject(Guid projectId)
    {
        if (currentUser.IsAdmin)
        {
            return true;
        }

        if (currentUser.UserId is not Guid userId)
        {
            return false;
        }

        var project =
            await projectService.GetByIdAsync(projectId);

        if (project is null)
        {
            return false;
        }

        if (project.CreatedByUserId == userId)
        {
            return true;
        }

        var members =
            await memberService.GetByProjectIdAsync(projectId);

        return members.Any(
            member => member.UserId == userId);
    }
}