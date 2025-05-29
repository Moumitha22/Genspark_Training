using System;

namespace BankManagementSystem.Models
{

    public class Account
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string AccountNumber { get; set; } = string.Empty;

        public string AccountType { get; set; } = string.Empty;

        public decimal Balance { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime OpenedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ClosedAt { get; set; }

        public User? User { get; set; }

        public ICollection<Transaction>? Transactions { get; set; }
    }

}