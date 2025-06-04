using ClinicManagementSystem.Contexts;
using ClinicManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Repositories
{
    public class UserRepository : Repository<string, User>
    {
        public UserRepository(ClinicContext context):base(context)
        {
            
        }
        public override async Task<User> Get(string key)
        {
            var user = await _clinicContext.Users.SingleOrDefaultAsync(u => u.Username == key);
            return user ?? throw new Exception($"No user with given user name {key}");
        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            var users = _clinicContext.Users;
            if (users.Count() == 0)
            {
                throw new Exception("No users in the database");
            }
            return await users.ToListAsync();
        }
            
    }
}