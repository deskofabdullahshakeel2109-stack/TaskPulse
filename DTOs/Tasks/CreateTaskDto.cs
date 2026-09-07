using System.ComponentModel.DataAnnotations;

namespace TaskPulse.Api.DTOs.Tasks;

public class CreateTaskDto
{
    public Guid ProjectId { get; set; }

    public Guid? AssignedToUserId { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Todo";

    [Required]
    [StringLength(50)]
    public string Priority { get; set; } = "Medium";

    public DateTime? DueDate { get; set; }
}