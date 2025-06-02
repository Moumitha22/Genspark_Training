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
            return await _clinicContext.Users.SingleOrDefaultAsync(u => u.Username == key);
        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            return await _clinicContext.Users.ToListAsync();
        }
            
    }
}