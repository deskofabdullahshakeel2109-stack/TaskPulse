using Microsoft.EntityFrameworkCore;
using TaskPulse.Api.Data;
using TaskPulse.Api.Models;
using TaskPulse.Api.Repositories.Interfaces;

namespace TaskPulse.Api.Repositories.ProjectMembers;

public class ProjectMemberRepository : IProjectMemberRepository
{
    private readonly ApplicationDbContext _context;
    public ProjectMemberRepository(ApplicationDbContext context) => _context = context;

    public Task<List<ProjectMember>> GetByProjectIdAsync(Guid projectId) =>
        _context.ProjectMembers.AsNoTracking()
            .Include(pm => pm.User).Include(pm => pm.Project)
            .Where(pm => pm.ProjectId == projectId)
            .OrderBy(pm => pm.JoinedAt).ToListAsync();

    public Task<ProjectMember?> GetByIdAsync(Guid id) =>
        _context.ProjectMembers.Include(pm => pm.User).Include(pm => pm.Project)
            .FirstOrDefaultAsync(pm => pm.Id == id);

    public Task<bool> ExistsAsync(Guid projectId, Guid userId) =>
        _context.ProjectMembers.AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

    public async Task<ProjectMember> CreateAsync(ProjectMember projectMember)
    {
        await _context.ProjectMembers.AddAsync(projectMember);
        await _context.SaveChangesAsync();
        return projectMember;
    }

    public async Task<ProjectMember?> UpdateAsync(ProjectMember projectMember)
    {
        var existing = await _context.ProjectMembers.FirstOrDefaultAsync(pm => pm.Id == projectMember.Id);
        if (existing is null) return null;
        existing.Role = projectMember.Role;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var member = await _context.ProjectMembers.FirstOrDefaultAsync(pm => pm.Id == id);
        if (member is null) return false;
        _context.ProjectMembers.Remove(member);
        await _context.SaveChangesAsync();
        return true;
    }
}
