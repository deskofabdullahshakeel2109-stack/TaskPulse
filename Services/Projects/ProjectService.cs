using TaskPulse.Api.DTOs.Projects;
using TaskPulse.Api.Models;
using TaskPulse.Api.Repositories.Interfaces;

namespace TaskPulse.Api.Services.Projects;

public class ProjectService(IProjectRepository repository) : IProjectService
{
    public async Task<List<ProjectResponseDto>> GetByUserIdAsync(Guid userId)
    {
        return (await repository.GetByUserIdAsync(userId))
            .Select(Map)
            .ToList();
    }
    public async Task<List<ProjectResponseDto>> GetAllAsync() =>
        (await repository.GetAllAsync()).Select(Map).ToList();

    public async Task<ProjectResponseDto?> GetByIdAsync(Guid id)
    {
        var project = await repository.GetByIdAsync(id);
        return project is null ? null : Map(project);
    }

    public async Task<ProjectResponseDto> CreateAsync(CreateProjectDto dto, Guid userId)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };
        return Map(await repository.CreateAsync(project));
    }

    public async Task<ProjectResponseDto?> UpdateAsync(Guid id, UpdateProjectDto dto)
    {
        var existing = await repository.GetByIdAsync(id);
        if (existing is null) return null;
        existing.Name = dto.Name.Trim();
        existing.Description = dto.Description?.Trim();
        existing.UpdatedAt = DateTime.UtcNow;
        var updated = await repository.UpdateAsync(existing);
        return updated is null ? null : Map(updated);
    }

    public Task<bool> DeleteAsync(Guid id) => repository.DeleteAsync(id);

    private static ProjectResponseDto Map(Project p) => new()
    {
        Id = p.Id, Name = p.Name, Description = p.Description,
        CreatedByUserId = p.CreatedByUserId, CreatedAt = p.CreatedAt, UpdatedAt = p.UpdatedAt
    };
}
