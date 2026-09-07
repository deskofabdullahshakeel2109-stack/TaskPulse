using System.ComponentModel.DataAnnotations;

namespace TaskPulse.Api.DTOs.Comments;

public class CreateCommentDto
{
    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;
}