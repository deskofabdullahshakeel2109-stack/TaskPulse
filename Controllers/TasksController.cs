using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskPulse.Api.DTOs.Tasks;
using TaskPulse.Api.Services.CurrentUser;
using TaskPulse.Api.Services.ProjectMembers;
using TaskPulse.Api.Services.Projects;
using TaskPulse.Api.Services.Tasks;

namespace TaskPulse.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/tasks")]
[Authorize]
public class TasksController(
    ITaskService taskService,
    IProjectService projectService,
    IProjectMemberService memberService,
    ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TaskResponseDto>>> GetByProject(
        Guid projectId,
        [FromQuery] TaskQueryDto query)
    {
        if (!await CanAccessProject(projectId))
        {
            return Forbid();
        }

        var project = await projectService.GetByIdAsync(projectId);

        if (project is null)
        {
            return NotFound(new
            {
                message = "Project not found."
            });
        }

        return Ok(
            await taskService.GetFilteredAsync(projectId, query));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskResponseDto>> GetById(
        Guid projectId,
        Guid id)
    {
        if (!await CanAccessProject(projectId))
        {
            return Forbid();
        }

        var task = await taskService.GetByIdAsync(id);

        if (task is null || task.ProjectId != projectId)
        {
            return NotFound(new
            {
                message = "Task not found."
            });
        }

        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponseDto>> Create(
        Guid projectId,
        CreateTaskDto dto)
    {
        if (!await CanManageTasks(projectId))
        {
            return Forbid();
        }

        if (await projectService.GetByIdAsync(projectId) is null)
        {
            return NotFound(new
            {
                message = "Project not found."
            });
        }

        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        try
        {
            var task = await taskService.CreateAsync(
                projectId,
                dto,
                userId);

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    projectId,
                    id = task.Id
                },
                task);
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
    public async Task<ActionResult<TaskResponseDto>> Update(
        Guid projectId,
        Guid id,
        UpdateTaskDto dto)
    {
        if (!await CanManageTasks(projectId))
        {
            return Forbid();
        }

        var existing = await taskService.GetByIdAsync(id);

        if (existing is null || existing.ProjectId != projectId)
        {
            return NotFound(new
            {
                message = "Task not found."
            });
        }

        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        try
        {
            var updated = await taskService.UpdateAsync(
                id,
                dto,
                userId);

            return updated is null
                ? NotFound(new
                {
                    message = "Task not found."
                })
                : Ok(updated);
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
        Guid projectId,
        Guid id)
    {
        if (!await CanManageTasks(projectId))
        {
            return Forbid();
        }

        var existing = await taskService.GetByIdAsync(id);

        if (existing is null || existing.ProjectId != projectId)
        {
            return NotFound(new
            {
                message = "Task not found."
            });
        }

        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var deleted = await taskService.DeleteAsync(
            id,
            userId);

        return deleted
            ? Ok(new
            {
                message = "Task deleted successfully."
            })
            : NotFound(new
            {
                message = "Task not found."
            });
    }

    private async Task<bool> CanManageTasks(Guid projectId)
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

        // Project creator can manage tasks.
        if (project.CreatedByUserId == userId)
        {
            return true;
        }

        // Project managers can manage tasks.
        var members =
            await memberService.GetByProjectIdAsync(projectId);

        var member = members.FirstOrDefault(
            m => m.UserId == userId);

        return member?.Role.Equals(
            "Manager",
            StringComparison.OrdinalIgnoreCase) == true;
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
            m => m.UserId == userId);
    }
}