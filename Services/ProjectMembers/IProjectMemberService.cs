using TaskPulse.Api.DTOs.ProjectMembers;

namespace TaskPulse.Api.Services.ProjectMembers;

public interface IProjectMemberService
{
    Task<List<ProjectMemberResponseDto>> GetByProjectIdAsync(Guid projectId);
    Task<ProjectMemberResponseDto?> GetByIdAsync(Guid id);
    Task<ProjectMemberResponseDto> AddMemberAsync(Guid projectId, AddProjectMemberDto dto);
    Task<ProjectMemberResponseDto?> UpdateRoleAsync(Guid id, UpdateProjectMemberRoleDto dto);
    Task<bool> RemoveMemberAsync(Guid id);
}
