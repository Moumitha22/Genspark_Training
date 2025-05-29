using System;

namespace BankManagementSystem.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = string.Empty;
        public decimal BalanceAfterTransaction { get; set; }

        public Account? Account { get; set; }
    }
}
