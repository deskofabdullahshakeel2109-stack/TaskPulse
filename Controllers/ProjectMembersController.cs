using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskPulse.Api.DTOs.ProjectMembers;
using TaskPulse.Api.Helpers;
using TaskPulse.Api.Services.CurrentUser;
using TaskPulse.Api.Services.Projects;
using TaskPulse.Api.Services.ProjectMembers;

namespace TaskPulse.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/members")]
[Authorize]
public class ProjectMembersController(
    IProjectMemberService memberService,
    IProjectService projectService,
    ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProjectMemberResponseDto>>> GetByProject(Guid projectId)
    {
        if (!await CanManageOrViewProject(projectId)) return Forbid();
        return Ok(await memberService.GetByProjectIdAsync(projectId));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectMemberResponseDto>> GetById(Guid projectId, Guid id)
    {
        if (!await CanManageOrViewProject(projectId)) return Forbid();
        var member = await memberService.GetByIdAsync(id);
        if (member is null || member.ProjectId != projectId)
            return NotFound(new { message = "Project member not found." });
        return Ok(member);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectMemberResponseDto>> Add(Guid projectId, AddProjectMemberDto dto)
    {
        if (!await CanManageProject(projectId)) return Forbid();

        try
        {
            var member = await memberService.AddMemberAsync(projectId, dto);
            return Ok(member);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProjectMemberResponseDto>> UpdateRole(
        Guid projectId, Guid id, UpdateProjectMemberRoleDto dto)
    {
        if (!await CanManageProject(projectId)) return Forbid();

        var existing = await memberService.GetByIdAsync(id);
        if (existing is null || existing.ProjectId != projectId)
            return NotFound(new { message = "Project member not found." });

        var updated = await memberService.UpdateRoleAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove(Guid projectId, Guid id)
    {
        if (!await CanManageProject(projectId)) return Forbid();

        var existing = await memberService.GetByIdAsync(id);
        if (existing is null || existing.ProjectId != projectId)
            return NotFound(new { message = "Project member not found." });

        await memberService.RemoveMemberAsync(id);
        return Ok(new { message = "Project member removed successfully." });
    }

    private async Task<bool> CanManageProject(Guid projectId)
    {
        if (currentUser.IsAdmin) return true;
        if (currentUser.UserId is not Guid userId) return false;
        var project = await projectService.GetByIdAsync(projectId);
        if (project is null) return false;
        if (project.CreatedByUserId == userId) return true;

        var members = await memberService.GetByProjectIdAsync(projectId);
        var member = members.FirstOrDefault(m => m.UserId == userId);
        return member?.Role.Equals("Manager", StringComparison.OrdinalIgnoreCase) == true;
    }

    private async Task<bool> CanManageOrViewProject(Guid projectId) =>
        await CanManageProject(projectId) ||
        (currentUser.UserId is Guid userId &&
         (await memberService.GetByProjectIdAsync(projectId)).Any(m => m.UserId == userId));
}
