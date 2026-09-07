using TaskPulse.Api.DTOs.Reports;

namespace TaskPulse.Api.Services.Reports;

public interface IReportService
{
    Task<ProjectReportResponseDto> GetProjectReportAsync(
        Guid projectId);
}