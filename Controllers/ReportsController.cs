using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskPulse.Api.DTOs.Reports;
using TaskPulse.Api.Services.CurrentUser;
using TaskPulse.Api.Services.ProjectMembers;
using TaskPulse.Api.Services.Projects;
using TaskPulse.Api.Services.Reports;

namespace TaskPulse.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/reports")]
[Authorize]
public class ReportsController(
    IReportService reportService,
    IProjectService projectService,
    IProjectMemberService memberService,
    ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ProjectReportResponseDto>>
        GetProjectReport(Guid projectId)
    {
        if (!await CanAccessProject(projectId))
        {
            var project =
                await projectService.GetByIdAsync(projectId);

            if (project is null)
            {
                return NotFound(new
                {
                    message = "Project not found."
                });
            }

            return Forbid();
        }

        var report =
            await reportService.GetProjectReportAsync(projectId);

        return Ok(report);
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