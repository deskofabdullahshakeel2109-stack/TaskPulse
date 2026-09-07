namespace TaskPulse.Api.Services.Dashboard;

public interface IDashboardService
{
    Task<DTOs.Dashboard.DashboardResponseDto> GetProjectDashboardAsync(
        Guid projectId);
}