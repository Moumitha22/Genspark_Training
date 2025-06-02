using System.Collections.Generic;
using System.Threading.Tasks;
using BankManagementSystem.Models;
using BankManagementSystem.Interfaces;
using BankManagementSystem.Models.DTOs;
using BankManagementSystem.Misc;

namespace BankManagementSystem.Services
{
    public class AccountService : IAccountService
    {
        private readonly IRepository<int, Account> _accountRepository;
        private readonly IRepository<int, User> _userRepository;
        private readonly AccountMapper _accountMapper;

        public AccountService(IRepository<int, Account> accountRepository, IRepository<int, User> userRepository)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _accountMapper = new AccountMapper();
        }

        public async Task<Account> CreateAccount(AccountAddRequestDto accountAddRequestDto)
        {
            try
            {
                var user = await _userRepository.Get(accountAddRequestDto.UserId);
                
                var account = _accountMapper.MapAccountAddRequestDtoToAccount(accountAddRequestDto);
                account = await _accountRepository.Add(account);
                account.AccountNumber = $"ACC{account.Id:D8}";

                // Update AccountNumber
                account = await _accountRepository.Update(account.Id, account);

                return account;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<ICollection<Account>> GetAllAccounts()
        {
            try
            {
                var accounts = await _accountRepository.GetAll();
                return accounts.ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<Account> GetAccountById(int accountId)
        {
            try
            {
                var account = await _accountRepository.Get(accountId);
                return account;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Account> GetAccountByAccountNumber(string accountNumber)
        {
            try
            {
                var account = await _accountRepository.GetAll();
                var matchedAccount = account.FirstOrDefault(a => a.AccountNumber == accountNumber);

                if (matchedAccount == null)
                    throw new Exception($"Account not found with number {accountNumber}");

                return matchedAccount;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<decimal> CheckBalance(string accountNumber)
        {
            try
            {
                var account = await GetAccountByAccountNumber(accountNumber);
                if (account == null)
                    throw new Exception($"Account not found with number {accountNumber}");

                return account.Balance;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> CloseAccount(string accountNumber)
        {
            try
            {
                var account = await GetAccountByAccountNumber(accountNumber);

                if (account == null)
                    throw new Exception($"Account not found with number {accountNumber}");

                if (account.Status == "Close")
                    throw new Exception("Account is already closed.");

                account.Status = "Close";
                await _accountRepository.Update(account.Id, account);

                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        

    }
}
