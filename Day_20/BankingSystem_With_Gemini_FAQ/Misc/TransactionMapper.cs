
using BankManagementSystem.Models;
using BankManagementSystem.Models.DTOs;

namespace BankManagementSystem.Misc
{
    public class TransactionMapper
    {
        public Transaction MapDepositDtoToTransaction(TransactionAddRequestDto transactionAddRequestDto, int accountId)
        {
            return new Transaction
            {
                AccountId = accountId,
                TransactionType = "Deposit",
                Amount = transactionAddRequestDto.Amount,
                Description = string.IsNullOrEmpty(transactionAddRequestDto.Description) ? "Deposit" : transactionAddRequestDto.Description,
                TransactionDate = DateTime.UtcNow,
                Status = "Success"
            };
        }

        public Transaction MapWithdrawDtoToTransaction(TransactionAddRequestDto transactionAddRequestDto, int accountId)
        {
            return new Transaction
            {
                AccountId = accountId,
                TransactionType = "Withdrawal",
                Amount = transactionAddRequestDto.Amount,
                Description = string.IsNullOrEmpty(transactionAddRequestDto.Description) ? "Withdrawal" : transactionAddRequestDto.Description,
                TransactionDate = DateTime.UtcNow,
                Status = "Success"
            };
        }

          public (Transaction debit, Transaction credit) MapTransferDtoToTransactions(TransactionTransferRequestDto dto, Account fromAccount, Account toAccount)
        {

            var debitTransaction = new Transaction
            {
                AccountId = fromAccount.Id,
                TransactionType = "Transfer (Debit)",
                Amount = dto.Amount,
                Description = string.IsNullOrEmpty(dto.Description) ? "Transfer to " + toAccount.AccountNumber : dto.Description,
                TransactionDate = DateTime.UtcNow,
                Status = "Success",
                BalanceAfterTransaction = fromAccount.Balance
            };

            var creditTransaction = new Transaction
            {
                AccountId = toAccount.Id,
                TransactionType = "Transfer (Credit)",
                Amount = dto.Amount,
                Description = string.IsNullOrEmpty(dto.Description) ? "Transfer from " + fromAccount.AccountNumber : dto.Description,
                TransactionDate = DateTime.UtcNow,
                Status = "Success",
                BalanceAfterTransaction = toAccount.Balance
            };

            return (debitTransaction, creditTransaction);
        }

    }
}
