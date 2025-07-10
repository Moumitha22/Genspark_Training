using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Models.Enums;

namespace PropFinderApi.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user);
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByEmailandRoleAsync(string email, UserRole role);
        Task<UserResponseDto?> UpdateUserAsync(Guid userId, UserUpdateRequestDto dto, Guid requesterId, string userRole);
        Task UpdateUserStatusAsync(Guid id, bool disable);
    }
}