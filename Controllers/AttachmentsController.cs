using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskPulse.Api.DTOs.Attachments;
using TaskPulse.Api.Services.Attachments;
using TaskPulse.Api.Services.CurrentUser;
using TaskPulse.Api.Services.ProjectMembers;
using TaskPulse.Api.Services.Projects;
using TaskPulse.Api.Services.Tasks;

namespace TaskPulse.Api.Controllers;

[ApiController]
[Route("api/tasks/{taskId:guid}/attachments")]
[Authorize]
public class AttachmentsController(
    IAttachmentService attachmentService,
    ITaskService taskService,
    IProjectService projectService,
    IProjectMemberService memberService,
    ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AttachmentResponseDto>>> GetByTask(
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
            await attachmentService.GetByTaskIdAsync(taskId));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AttachmentResponseDto>> GetById(
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

        var attachment =
            await attachmentService.GetByIdAsync(id);

        if (attachment is null ||
            attachment.TaskId != taskId)
        {
            return NotFound(new
            {
                message = "Attachment not found."
            });
        }

        return Ok(attachment);
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(
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

        var attachment =
            await attachmentService.GetByIdAsync(id);

        if (attachment is null ||
            attachment.TaskId != taskId)
        {
            return NotFound(new
            {
                message = "Attachment not found."
            });
        }

        var file =
            await attachmentService.GetFileAsync(id);

        if (file is null)
        {
            return NotFound(new
            {
                message = "Attachment file not found."
            });
        }

        return File(
            file.Value.Stream,
            file.Value.ContentType,
            file.Value.FileName);
    }

    [HttpPost]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<AttachmentResponseDto>> Upload(
        Guid taskId,
        [FromForm] AttachmentUploadDto dto)
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
            var attachment =
                await attachmentService.UploadAsync(
                    taskId,
                    userId,
                    dto.File);

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    taskId,
                    id = attachment.Id
                },
                attachment);
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

        var attachment =
            await attachmentService.GetByIdAsync(id);

        if (attachment is null ||
            attachment.TaskId != taskId)
        {
            return NotFound(new
            {
                message = "Attachment not found."
            });
        }

        var deleted =
            await attachmentService.DeleteAsync(id);

        return deleted
            ? Ok(new
            {
                message = "Attachment deleted successfully."
            })
            : NotFound(new
            {
                message = "Attachment not found."
            });
    }

    private async Task<bool> CanAccessProject(
        Guid projectId)
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