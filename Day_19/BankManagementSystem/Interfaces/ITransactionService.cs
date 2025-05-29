using BankManagementSystem.Models;
using BankManagementSystem.Models.DTOs;

namespace BankManagementSystem.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> Deposit(TransactionAddRequestDto transactionAddRequestDto);
        Task<Transaction> Withdraw(TransactionAddRequestDto transactionAddRequestDto);
        Task<decimal> CheckBalance(int accountId);
    }
}