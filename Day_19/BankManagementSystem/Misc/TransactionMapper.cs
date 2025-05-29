
using BankManagementSystem.Models;
using BankManagementSystem.Models.DTOs;

namespace BankManagementSystem.Misc
{
    public class TransactionMapper
    {
        public Transaction MapDepositDtoToTransaction(TransactionAddRequestDto transactionAddRequestDto)
        {
            return new Transaction
            {
                AccountId = transactionAddRequestDto.AccountId,
                TransactionType = "Deposit",
                Amount = transactionAddRequestDto.Amount,
                Description = string.IsNullOrEmpty(transactionAddRequestDto.Description) ? "Deposit" : transactionAddRequestDto.Description,
                TransactionDate = DateTime.UtcNow,
                Status = "Success"
            };
        }

        public Transaction MapWithdrawDtoToTransaction(TransactionAddRequestDto transactionAddRequestDto)
        {
            return new Transaction
            {
                AccountId = transactionAddRequestDto.AccountId,
                TransactionType = "Withdrawal",
                Amount = transactionAddRequestDto.Amount,
                Description = string.IsNullOrEmpty(transactionAddRequestDto.Description) ? "Withdrawal" : transactionAddRequestDto.Description,
                TransactionDate = DateTime.UtcNow,
                Status = "Success"
            };
        }
    }
}
