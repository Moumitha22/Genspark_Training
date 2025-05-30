using System;
using BankManagementSystem.Models;
using BankManagementSystem.Models.DTOs;

namespace BankManagementSystem.Interfaces
{

    public interface IAccountService
    {
        public Task<Account> CreateAccount(AccountAddRequestDto accountAddRequestDto);
        public Task<ICollection<Account>> GetAllAccounts();
        public Task<Account> GetAccountById(int accountId);
        public Task<Account> GetAccountByAccountNumber(string accountNumber);
        public Task<bool> CloseAccount(string accountNumber);
        public Task<decimal> CheckBalance(string accountNumber);

    }
}