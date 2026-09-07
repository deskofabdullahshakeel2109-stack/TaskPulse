using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskPulse.Api.DTOs.Projects;
using TaskPulse.Api.Helpers;
using TaskPulse.Api.Services.CurrentUser;
using TaskPulse.Api.Services.Projects;

namespace TaskPulse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController(IProjectService projectService, ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProjectResponseDto>>> GetAll() =>
        Ok(await projectService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectResponseDto>> GetById(Guid id)
    {
        var project = await projectService.GetByIdAsync(id);
        return project is null ? NotFound(new { message = "Project not found." }) : Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponseDto>> Create(CreateProjectDto dto)
    {
        if (currentUser.UserId is not Guid userId) return Unauthorized();
        var project = await projectService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProjectResponseDto>> Update(Guid id, UpdateProjectDto dto)
    {
        var project = await projectService.GetByIdAsync(id);
        if (project is null) return NotFound(new { message = "Project not found." });
        if (!currentUser.IsAdmin && currentUser.UserId != project.CreatedByUserId) return Forbid();

        var updated = await projectService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var project = await projectService.GetByIdAsync(id);
        if (project is null) return NotFound(new { message = "Project not found." });
        if (!currentUser.IsAdmin && currentUser.UserId != project.CreatedByUserId) return Forbid();

        await projectService.DeleteAsync(id);
        return Ok(new { message = "Project deleted successfully." });
    }
}
