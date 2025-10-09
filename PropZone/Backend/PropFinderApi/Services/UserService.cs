using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Mappers;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Models.Enums;

namespace PropFinderApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IListerProfileRepository _listerProfileRepository;
        private readonly UserMapper _userMapper;

        public UserService(IUserRepository userRepository, IListerProfileRepository listerProfileRepository)
        {
            _userRepository = userRepository;
            _listerProfileRepository = listerProfileRepository;
            _userMapper = new UserMapper();
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

        public async Task<User?> GetUserByEmailandRoleAsync(string email, UserRole role)
        {
            email = email.Trim().ToLowerInvariant();
            return await _userRepository.GetByEmailandRoleAsync(email, role);
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAll();

            return users
                .Select(u => _userMapper.MapUserToUserResponseDto(u))
                .ToList();
        }

        public async Task<UserResponseDto?> UpdateUserAsync(Guid userId, UserUpdateRequestDto dto, Guid requesterId, string userRole)
        {
            if (userRole != "Admin" && userId != requesterId)
                throw new UnauthorizedException("You are not authorized to update this data.");

            var user = await _userRepository.Get(userId);

            user.Name = dto.Name?.Trim() ?? user.Name;
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            var updated = await _userRepository.Update(user.Id, user);
            return _userMapper.MapUserToUserResponseDto(updated);
        }

        public async Task UpdateUserStatusAsync(Guid id, bool disable)
        {
            var user = await _userRepository.Get(id);

            user.IsDeleted = disable;
            user.UpdatedAt = DateTime.UtcNow;

            if (user.Role == UserRole.Lister)
            {
                var profile = await _listerProfileRepository.GetByUserIdAsync(user.Id);
                if (profile != null)
                {
                    profile.IsDeleted = disable;
                    profile.UpdatedAt = DateTime.UtcNow;
                    await _listerProfileRepository.Update(profile.Id, profile);
                }
            }

            await _userRepository.Update(user.Id, user);
        }

    }
}