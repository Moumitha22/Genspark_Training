using ChienVHShopOnline.Data;
using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories
{
    public class UserRepository : Repository<int, User>, IUserRepository
    {
        public UserRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<User> Get(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            return user ??
                throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            return await _dbContext.Users
            .OrderBy(c => c.Username)
            .ToListAsync();
        }
        
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbContext.Users
                .SingleOrDefaultAsync(u => u.Email == email);
        }

    }
}
