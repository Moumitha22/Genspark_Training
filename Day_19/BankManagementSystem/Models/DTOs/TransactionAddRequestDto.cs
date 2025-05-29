namespace BankManagementSystem.Models.DTOs
{
    public class TransactionAddRequestDto
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }

}