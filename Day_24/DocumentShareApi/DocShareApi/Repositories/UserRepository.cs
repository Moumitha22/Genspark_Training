using DocShareApi.Data;
using DocShareApi.Interfaces;
using DocShareApi.Models;

namespace DocShareApi.Repositories
{
    public class UserRepository : IRepository<string, User>
{
    private readonly DocShareDbContext _context;
    public UserRepository(DocShareDbContext context)
    {
        _context = context;
    }

    public async Task<User> Add(User entity)
    {
        _context.Users.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<User> Delete(string id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return null;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> Get(string id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return _context.Users;
    }

    public async Task<User> Update(string id, User entity)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return null;

        user.Name = entity.Name;
        user.Role = entity.Role;
        user.Status = entity.Status;

        await _context.SaveChangesAsync();
        return user;
    }
}

}