using System.ComponentModel.DataAnnotations;

namespace TaskPulse.Api.DTOs.Projects;

public class CreateProjectDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }
}