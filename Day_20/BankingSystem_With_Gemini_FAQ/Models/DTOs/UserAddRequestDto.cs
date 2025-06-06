namespace BankManagementSystem.Models.DTOs
{
    public class UserAddRequestDto
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }
    }
}