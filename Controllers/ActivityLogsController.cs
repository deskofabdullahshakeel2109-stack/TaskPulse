using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskPulse.Api.DTOs.ActivityLogs;
using TaskPulse.Api.Services.ActivityLogs;
using TaskPulse.Api.Services.CurrentUser;
using TaskPulse.Api.Services.ProjectMembers;
using TaskPulse.Api.Services.Projects;

namespace TaskPulse.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/activity-logs")]
[Authorize]
public class ActivityLogsController(
    IActivityLogService activityLogService,
    IProjectService projectService,
    IProjectMemberService memberService,
    ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ActivityLogResponseDto>>> GetByProjectId(
        Guid projectId)
    {
        if (!await CanAccess(projectId))
        {
            return Forbid();
        }

        var logs = await activityLogService
            .GetByProjectIdAsync(projectId);

        return Ok(logs);
    }

    private async Task<bool> CanAccess(Guid projectId)
    {
        if (currentUser.IsAdmin)
        {
            return true;
        }

        if (currentUser.UserId is not Guid userId)
        {
            return false;
        }

        var project = await projectService.GetByIdAsync(projectId);

        if (project is null)
        {
            return false;
        }

        if (project.CreatedByUserId == userId)
        {
            return true;
        }

        var members = await memberService
            .GetByProjectIdAsync(projectId);

        return members.Any(m => m.UserId == userId);
    }
}