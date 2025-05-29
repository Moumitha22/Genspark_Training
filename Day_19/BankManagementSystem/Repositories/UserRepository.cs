using BankManagementSystem.Contexts;
using BankManagementSystem.Models;
using Microsoft.EntityFrameworkCore;


namespace BankManagementSystem.Repositories
{

    public class UserRepository : Repository<int, User>
    {
        public UserRepository(BankContext bankContext) : base(bankContext)
        {
        }

        public override async Task<User> Get(int key)
        {
            var user = await _bankContext.Users.SingleOrDefaultAsync(a => a.Id == key);
            return user ?? throw new Exception($"No user with given ID {key}");
        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            var users = _bankContext.Users;
            if (users.Count() == 0)
            {
                throw new Exception("No users in the database");
            }
            return await users.ToListAsync();
        }

    }
}
