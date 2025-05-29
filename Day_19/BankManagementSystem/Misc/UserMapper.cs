using BankManagementSystem.Models;
using BankManagementSystem.Models.DTOs;

namespace BankManagementSystem.Misc
{
    public class UserMapper
    {
        public User MapUserAddRequestDtoToUser(UserAddRequestDto addRequestDto)
        {
            User user = new User
            {
                Name = addRequestDto.Name,
                Email = addRequestDto.Email,
                PhoneNumber = addRequestDto.PhoneNumber,
                Address = addRequestDto.Address,
                DateOfBirth = DateTime.SpecifyKind(addRequestDto.DateOfBirth, DateTimeKind.Utc),
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };
            return user;
        }
    }
}