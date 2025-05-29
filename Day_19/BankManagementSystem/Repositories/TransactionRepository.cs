using BankManagementSystem.Contexts;
using BankManagementSystem.Models;
using Microsoft.EntityFrameworkCore;


namespace BankManagementSystem.Repositories
{

    public class TransactionRepository : Repository<int, Transaction>
    {
        public TransactionRepository(BankContext bankContext) : base(bankContext)
        {
        }

        public override async Task<Transaction> Get(int key)
        {
            var transaction = await _bankContext.Transactions.SingleOrDefaultAsync(t => t.Id == key);
            return transaction ?? throw new Exception($"No Transaction with given ID {key}");
        }

        public override async Task<IEnumerable<Transaction>> GetAll()
        {
            var transactions = _bankContext.Transactions;
            if (transactions.Count() == 0)
            {
                throw new Exception("No Transactions in the database");
            }
            return await transactions.ToListAsync();
        }

    }
}
