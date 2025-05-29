using System;
using BankManagementSystem.Models;
using BankManagementSystem.Models.DTOs;

namespace BankManagementSystem.Interfaces
{

    public interface IAccountService
    {
        Task<Account> GetAccountById(int accountId);
        Task<ICollection<Account>> GetAllAccounts();
        Task<Account> CreateAccount(AccountAddRequestDto accountAddRequestDto);
    }
}