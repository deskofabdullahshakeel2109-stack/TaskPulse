using TaskPulse.Api.DTOs.Tasks;

namespace TaskPulse.Api.Services.Tasks;

public interface ITaskService
{
    Task<List<TaskResponseDto>> GetAllAsync();

    Task<List<TaskResponseDto>> GetByProjectIdAsync(Guid projectId);

    Task<List<TaskResponseDto>> GetFilteredAsync(
        Guid projectId,
        TaskQueryDto query);

    Task<TaskResponseDto?> GetByIdAsync(Guid id);

    Task<TaskResponseDto> CreateAsync(
        Guid projectId,
        CreateTaskDto dto,
        Guid createdByUserId);

    Task<TaskResponseDto?> UpdateAsync(
        Guid id,
        UpdateTaskDto dto,
        Guid updatedByUserId);

    Task<bool> DeleteAsync(
        Guid id,
        Guid deletedByUserId);
}