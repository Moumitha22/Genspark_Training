using ChienVHShopOnline.Models;
using ChienVHShopOnline.Models.DTOs;

namespace ChienVHShopOnline.Interfaces
{
    public interface IUserService
    {
        // Task<UserResponseDto> CreateUser(UserRequestDto userDto);
        Task<UserResponseDto> CreateUser(User user);
        Task<IEnumerable<UserResponseDto>> GetAllUsers();
        Task<UserResponseDto> GetUserById(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<UserResponseDto> UpdateUser(int id, UserUpdateDto userDto);
        Task<UserResponseDto> DeleteUser(int id);
    }
}
