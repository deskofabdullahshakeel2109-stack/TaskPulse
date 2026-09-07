using TaskPulse.Api.Models;

namespace TaskPulse.Api.Repositories.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync();

    Task<List<Project>> GetByUserIdAsync(Guid userId);

    Task<Project?> GetByIdAsync(Guid id);

    Task<Project> CreateAsync(Project project);

    Task<Project?> UpdateAsync(Project project);

    Task<bool> DeleteAsync(Guid id);
}