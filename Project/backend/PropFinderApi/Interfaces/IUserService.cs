using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> UpdateUserAsync(Guid userId, UserUpdateRequestDto dto, Guid requesterId, string userRole);
        Task DeleteUserAsync(Guid id);
    }
}