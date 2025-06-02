using BankManagementSystem.Models;
using BankManagementSystem.Models.DTOs;

namespace BankManagementSystem.Interfaces
{
    public interface ITransactionService
    {
        public Task<Transaction> Deposit(TransactionAddRequestDto transactionAddRequestDto);
        public Task<Transaction> Withdraw(TransactionAddRequestDto transactionAddRequestDto);
        public Task<Transaction> Transfer(TransactionTransferRequestDto transactionTransferRequestDto);
        public Task<IEnumerable<Transaction>> GetTransactionsByAccountNumber(string accountNumber);
    }
}