using BankManagementSystem.Contexts;
using BankManagementSystem.Models;
using Microsoft.EntityFrameworkCore;


namespace BankManagementSystem.Repositories
{

    public class AccountRepository : Repository<int, Account>
    {
        public AccountRepository(BankContext bankContext) : base(bankContext)
        {
        }

        public override async Task<Account> Get(int key)
        {
            var account = await _bankContext.Accounts.SingleOrDefaultAsync(a => a.Id == key);
            return account ?? throw new Exception($"No account with given ID {key}");
        }

        public override async Task<IEnumerable<Account>> GetAll()
        {
            var accounts = _bankContext.Accounts;
            if (accounts.Count() == 0)
            {
                throw new Exception("No accounts in the database");
            }
            return await accounts.ToListAsync();
        }

    }
}
