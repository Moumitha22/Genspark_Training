using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Mappers
{
    public class UserMapper
    {
        public User MapRegisterRequestDtoToUser(UserRegisterRequestDto dto, string passwordHash)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Name = SanitizeName(dto.Name),
                Email = dto.Email.Trim().ToLowerInvariant(),
                Role = dto.Role,
                PasswordHash = passwordHash,
                PhoneNumber = dto.PhoneNumber ?? null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
        }
        public UserLoginResponseDto MapUserToLoginResponseDto(User user, string accessToken, string refreshToken)
        {
            return new UserLoginResponseDto
            {
                Username = user.Email,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public UserResponseDto MapUserToUserResponseDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                isDeleted = user.IsDeleted
            };
        }


        private string SanitizeName(string name)
        {
            return string.Join(" ", name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
