using BankManagementSystem.Models;
using BankManagementSystem.Models.DTOs;

namespace BankManagementSystem.Misc
{
    public class AccountMapper
    {
        public Account MapAccountAddRequestDtoToAccount(AccountAddRequestDto dto)
        {
            return new Account
            {
                UserId = dto.UserId,
                AccountType = dto.AccountType,
                Status = "Open",
                Balance = 0, 
                OpenedAt = DateTime.UtcNow
            };
        }
    }
}
