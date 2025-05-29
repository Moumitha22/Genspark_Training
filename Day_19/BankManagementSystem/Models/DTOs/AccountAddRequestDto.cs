using System;
using System.ComponentModel.DataAnnotations;

namespace BankManagementSystem.Models.DTOs
{
    public class AccountAddRequestDto
    {
        public int UserId { get; set; }
        public string AccountType { get; set; } = string.Empty;
    }
}
