namespace BankManagementSystem.Models.DTOs
{
    public class TransactionAddRequestDto
    {
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }

}