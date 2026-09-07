using System.ComponentModel.DataAnnotations;

namespace TaskPulse.Api.DTOs.ProjectMembers;

public class AddProjectMemberDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required, MaxLength(50)]
    [RegularExpression(
        "^(Member|Manager)$",
        ErrorMessage = "Role must be either Member or Manager.")]
    public string Role { get; set; } = "Member";
}