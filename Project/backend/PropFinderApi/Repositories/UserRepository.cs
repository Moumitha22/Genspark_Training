using Microsoft.EntityFrameworkCore;
using PropFinderApi.Models;
using PropFinderApi.Contexts;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;

namespace PropFinderApi.Repositories
{
    public class UserRepository : Repository<Guid, User>, IUserRepository
    {
        public UserRepository(PropFinderDbContext context) : base(context) { }

        public override async Task<User> Get(Guid id)
        {
            var user = await _propFinderDbContext.Users.SingleOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
            return user ?? throw new NotFoundException($"User with id '{id}' not found");
        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            return await _propFinderDbContext.Users
                .Where(u => !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _propFinderDbContext.Users
                .SingleOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }
        
        public async Task<User?> GetWithProfileAsync(Guid id)
        {
            return await _propFinderDbContext.Users
                .Include(u => u.AgentProfile)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

    }
}
