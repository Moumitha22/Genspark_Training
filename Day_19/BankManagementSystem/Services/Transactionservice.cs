using BankManagementSystem.Models;
using BankManagementSystem.Models.DTOs;
using BankManagementSystem.Misc;
using BankManagementSystem.Interfaces;
using Microsoft.EntityFrameworkCore;
using BankManagementSystem.Contexts;

namespace BankManagementSystem.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly BankContext _context;
        private readonly TransactionMapper _transactionMapper;

        public TransactionService(BankContext context)
        {
            _context = context;
            _transactionMapper = new TransactionMapper();
        }

        public async Task<Transaction> Deposit(TransactionAddRequestDto transactionAddRequestDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == transactionAddRequestDto.AccountNumber);
                if (account == null)
                    throw new Exception($"Account not found with accountNumber {transactionAddRequestDto.AccountNumber}");

                if (transactionAddRequestDto.Amount <= 0)
                    throw new Exception("Deposit amount must be greater than zero.");

                account.Balance += transactionAddRequestDto.Amount;

                var txn = _transactionMapper.MapDepositDtoToTransaction(transactionAddRequestDto, account.Id);
                txn.BalanceAfterTransaction = account.Balance;

                await _context.Transactions.AddAsync(txn);
                _context.Accounts.Update(account);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return txn;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Transaction> Withdraw(TransactionAddRequestDto transactionAddRequestDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                 var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == transactionAddRequestDto.AccountNumber);
                if (account == null)
                    throw new Exception($"Account not found with accountNumber {transactionAddRequestDto.AccountNumber}");

                if (transactionAddRequestDto.Amount <= 0)
                    throw new Exception("Withdrawal amount must be greater than zero.");

                if (account.Balance < transactionAddRequestDto.Amount)
                    throw new Exception("Insufficient balance.");

                account.Balance -= transactionAddRequestDto.Amount;

                var txn = _transactionMapper.MapWithdrawDtoToTransaction(transactionAddRequestDto, account.Id);
                txn.BalanceAfterTransaction = account.Balance;

                await _context.Transactions.AddAsync(txn);
                _context.Accounts.Update(account);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return txn;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Transaction> Transfer(TransactionTransferRequestDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var fromAccount = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountNumber == dto.FromAccountNumber);
                var toAccount = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountNumber == dto.ToAccountNumber);

                if (fromAccount == null)
                    throw new Exception($"From account not found with number {dto.FromAccountNumber}");

                if (toAccount == null)
                    throw new Exception($"To account not found with number {dto.ToAccountNumber}");

                if (dto.Amount <= 0)
                    throw new Exception("Transfer amount must be greater than zero.");

                if (fromAccount.Balance < dto.Amount)
                    throw new Exception("Insufficient balance in the source account.");

                // Update balances
                fromAccount.Balance -= dto.Amount;
                toAccount.Balance += dto.Amount;

                // Create transaction records
                var (debitTransaction, creditTransaction) = _transactionMapper.MapTransferDtoToTransactions(dto, fromAccount, toAccount);

                await _context.Transactions.AddRangeAsync(debitTransaction, creditTransaction);
                _context.Accounts.Update(fromAccount);
                _context.Accounts.Update(toAccount);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return debitTransaction;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByAccountNumber(string accountNumber)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
            if (account == null)
                throw new Exception($"Account not found with number {accountNumber}");

            var transactions = await _context.Transactions
                .Where(t => t.AccountId == account.Id)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return transactions;
        }


    }
}
