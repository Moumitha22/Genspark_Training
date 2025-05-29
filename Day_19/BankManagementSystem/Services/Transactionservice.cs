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
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == transactionAddRequestDto.AccountId);
                if (account == null)
                    throw new Exception($"Account not found with Id {transactionAddRequestDto.AccountId}");

                if (transactionAddRequestDto.Amount <= 0)
                    throw new Exception("Deposit amount must be greater than zero.");

                account.Balance += transactionAddRequestDto.Amount;

                var txn = _transactionMapper.MapDepositDtoToTransaction(transactionAddRequestDto);
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
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == transactionAddRequestDto.AccountId);
                if (account == null)
                    throw new Exception($"Account not found with Id {transactionAddRequestDto.AccountId}");

                if (transactionAddRequestDto.Amount <= 0)
                    throw new Exception("Withdrawal amount must be greater than zero.");

                if (account.Balance < transactionAddRequestDto.Amount)
                    throw new Exception("Insufficient balance.");

                account.Balance -= transactionAddRequestDto.Amount;

                var txn = _transactionMapper.MapWithdrawDtoToTransaction(transactionAddRequestDto);
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

        public async Task<decimal> CheckBalance(int accountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null)
                throw new Exception($"Account not found with Id {accountId}");

            return account.Balance;
        }
    }
}
