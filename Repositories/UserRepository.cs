using Microsoft.EntityFrameworkCore;
using TaskPulse.Api.Data;
using TaskPulse.Api.Models;
using TaskPulse.Api.Repositories.Interfaces;

namespace TaskPulse.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    public UserRepository(ApplicationDbContext context) => _context = context;

    public Task<List<User>> GetAllAsync() => _context.Users.AsNoTracking().OrderBy(u => u.FullName).ToListAsync();
    public Task<User?> GetByIdAsync(Guid id) => _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
    public Task<User?> GetByEmailAsync(string email) =>
        _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());
    public Task<bool> EmailExistsAsync(string email) =>
        _context.Users.AnyAsync(u => u.Email == email.ToLowerInvariant());

    public async Task<User> CreateAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}
