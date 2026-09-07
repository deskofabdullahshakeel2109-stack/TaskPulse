using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskPulse.Api.DTOs.Dashboard;
using TaskPulse.Api.Services.CurrentUser;
using TaskPulse.Api.Services.Dashboard;
using TaskPulse.Api.Services.ProjectMembers;
using TaskPulse.Api.Services.Projects;

namespace TaskPulse.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/dashboard")]
[Authorize]
public class DashboardController(
    IDashboardService dashboardService,
    IProjectService projectService,
    IProjectMemberService memberService,
    ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardResponseDto>> GetDashboard(
        Guid projectId)
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

        var dashboard =
            await dashboardService.GetProjectDashboardAsync(projectId);

        return Ok(dashboard);
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

        var project = await projectService.GetByIdAsync(projectId);

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

        return members.Any(m => m.UserId == userId);
    }
}