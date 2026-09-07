using Microsoft.EntityFrameworkCore;
using TaskPulse.Api.Data;
using TaskPulse.Api.Models;
using TaskPulse.Api.Repositories.Interfaces;

namespace TaskPulse.Api.Repositories.Projects;

public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Project>> GetAllAsync()
    {
        return _context.Projects
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public Task<List<Project>> GetByUserIdAsync(Guid userId)
    {
        return _context.Projects
            .AsNoTracking()
            .Where(p => p.CreatedByUserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public Task<Project?> GetByIdAsync(Guid id)
    {
        return _context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Project> CreateAsync(Project project)
    {
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        return project;
    }

    public async Task<Project?> UpdateAsync(Project project)
    {
        var existing = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == project.Id);

        if (existing is null)
            return null;

        existing.Name = project.Name;
        existing.Description = project.Description;
        existing.UpdatedAt = project.UpdatedAt;

        await _context.SaveChangesAsync();

        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project is null)
            return false;

        _context.Projects.Remove(project);

        await _context.SaveChangesAsync();

        return true;
    }
}