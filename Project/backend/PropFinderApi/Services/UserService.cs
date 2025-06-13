using System.Security.Claims;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Models.Enums;

namespace PropFinderApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAgentProfileRepository _agentProfileRepository;

        public UserService(IUserRepository userRepository, IAgentProfileRepository agentProfileRepository)
        {
            _userRepository = userRepository;
            _agentProfileRepository = agentProfileRepository;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            return await _userRepository.Add(user);
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _userRepository.Get(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            email = email.Trim().ToLowerInvariant();
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAll();
        }

        public async Task<User?> UpdateUserAsync(Guid userId, UserUpdateRequestDto dto, Guid requesterId, string userRole)
        {
            if(userRole != "Admin"  && userId != requesterId)
                throw new UnauthorizedException("You are not authorized to update this data.");

            var user = await _userRepository.Get(userId);

            user.Name = dto.Name?.Trim() ?? user.Name;
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            return await _userRepository.Update(user.Id, user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.Get(id);
            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;
            if (user.Role == UserRole.Agent)
            {

                var profile = await _agentProfileRepository.GetByUserIdAsync(user.Id);
                if (profile != null && !profile.IsDeleted)
                {
                    profile.IsDeleted = true;
                    profile.UpdatedAt = DateTime.UtcNow;
                    await _agentProfileRepository.Update(profile.Id, profile);
                }
            }
            await _userRepository.Update(user.Id, user);
        }
    }
}
