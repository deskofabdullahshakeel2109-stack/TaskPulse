using TaskPulse.Api.Models;

namespace TaskPulse.Api.Repositories.Interfaces;

public interface IProjectMemberRepository
{
    Task<List<ProjectMember>> GetByProjectIdAsync(Guid projectId);
    Task<ProjectMember?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid projectId, Guid userId);
    Task<ProjectMember> CreateAsync(ProjectMember projectMember);
    Task<ProjectMember?> UpdateAsync(ProjectMember projectMember);
    Task<bool> DeleteAsync(Guid id);
}
