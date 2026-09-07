using TaskPulse.Api.DTOs.ProjectMembers;
using TaskPulse.Api.Models;
using TaskPulse.Api.Repositories.Interfaces;

namespace TaskPulse.Api.Services.ProjectMembers;

public class ProjectMemberService : IProjectMemberService
{
    private readonly IProjectMemberRepository _projectMemberRepository;
    private readonly IUserRepository _userRepository;

    public ProjectMemberService(
        IProjectMemberRepository projectMemberRepository,
        IUserRepository userRepository)
    {
        _projectMemberRepository = projectMemberRepository;
        _userRepository = userRepository;
    }

    public async Task<List<ProjectMemberResponseDto>> GetByProjectIdAsync(Guid projectId)
    {
        var members = await _projectMemberRepository.GetByProjectIdAsync(projectId);
        return members.Select(MapToDto).ToList();
    }

    public async Task<ProjectMemberResponseDto?> GetByIdAsync(Guid id)
    {
        var member = await _projectMemberRepository.GetByIdAsync(id);
        return member is null ? null : MapToDto(member);
    }

    public async Task<ProjectMemberResponseDto> AddMemberAsync(Guid projectId, AddProjectMemberDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId);
        if (user is null) throw new InvalidOperationException("User not found.");

        if (await _projectMemberRepository.ExistsAsync(projectId, dto.UserId))
            throw new InvalidOperationException("User is already a member of this project.");

        var role = string.IsNullOrWhiteSpace(dto.Role)
    ? "Member"
    : dto.Role.Trim();

        ValidateRole(role);
        var member = new ProjectMember
        {
            Id = Guid.NewGuid(), ProjectId = projectId, UserId = dto.UserId,
            Role = role, JoinedAt = DateTime.UtcNow
        };
        var created = await _projectMemberRepository.CreateAsync(member);
        var result = await _projectMemberRepository.GetByIdAsync(created.Id);
        return MapToDto(result!);
    }

    public async Task<ProjectMemberResponseDto?> UpdateRoleAsync(Guid id, UpdateProjectMemberRoleDto dto)
    {
        var member = await _projectMemberRepository.GetByIdAsync(id);
        if (member is null) return null;
        var role = dto.Role.Trim();

        ValidateRole(role);

        member.Role = role;
        var updated = await _projectMemberRepository.UpdateAsync(member);
        if (updated is null) return null;
        var result = await _projectMemberRepository.GetByIdAsync(id);
        return MapToDto(result!);
    }

    public Task<bool> RemoveMemberAsync(Guid id) => _projectMemberRepository.DeleteAsync(id);

    private static ProjectMemberResponseDto MapToDto(ProjectMember member) => new()
    {
        Id = member.Id, ProjectId = member.ProjectId, UserId = member.UserId,
        FullName = member.User?.FullName ?? string.Empty, Email = member.User?.Email ?? string.Empty,
        Role = member.Role, JoinedAt = member.JoinedAt
    };

    private static void ValidateRole(string role)
    {
        var validRoles = new[]
        {
        "Member",
        "Manager"
    };

        if (!validRoles.Contains(
            role,
            StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Invalid project member role. Allowed values: {string.Join(", ", validRoles)}.");
        }
    }
}
