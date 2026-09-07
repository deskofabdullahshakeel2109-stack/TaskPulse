using TaskPulse.Api.DTOs.Projects;

namespace TaskPulse.Api.Services.Projects;

public interface IProjectService
{
    Task<List<ProjectResponseDto>> GetAllAsync();

    Task<List<ProjectResponseDto>> GetByUserIdAsync(Guid userId);

    Task<ProjectResponseDto?> GetByIdAsync(Guid id);

    Task<ProjectResponseDto> CreateAsync(
        CreateProjectDto dto,
        Guid userId);

    Task<ProjectResponseDto?> UpdateAsync(
        Guid id,
        UpdateProjectDto dto);

    Task<bool> DeleteAsync(Guid id);
}