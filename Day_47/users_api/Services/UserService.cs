using users_api.Models;
using users_api.Repositories;
using users_api.Interfaces;

namespace users_api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            await _repo.AddAsync(user);
            return user;
        }

        public async Task<bool> UpdateUserAsync(int id, User user)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return false;

            // Update properties
            existing.Name = user.Name;
            existing.Email = user.Email;

            await _repo.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return false;

            await _repo.DeleteAsync(id);
            return true;
        }
    }
}
