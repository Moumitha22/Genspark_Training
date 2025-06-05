using DocShareApi.Interfaces;
using DocShareApi.Models;
using DocShareApi.Models.DTOs;
using Microsoft.Extensions.Logging;

namespace DocShareApi.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ITokenService _tokenService;
        private readonly IEncryptionService _encryptionService;
        private readonly IRepository<string, User> _userRepository;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            ITokenService tokenService,
            IEncryptionService encryptionService,
            IRepository<string, User> userRepository,
            ILogger<AuthenticationService> logger)
        {
            _tokenService = tokenService;
            _encryptionService = encryptionService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<string> Register(RegisterRequestDto dto)
        {
            var existing = await _userRepository.Get(dto.Email);
            if (existing != null)
                throw new Exception("User already exists");

            var encrypted = await _encryptionService.HashPassword(new EncryptModel
            {
                Data = dto.Password
            });

            var newUser = new User
            {
                Email = dto.Email,
                Name = dto.Name,
                Role = dto.Role,
                Status = "Active",
                Password = encrypted.HashedData,
                HashKey = encrypted.HashKey
            };

            await _userRepository.Add(newUser);
            return "Registration successful";
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto dto)
        {
            var dbUser = await _userRepository.Get(dto.Email);
            if (dbUser == null)
            {
                throw new Exception($"User not found: {dto.Email}");
            }

            var encryptedData = await _encryptionService.HashPassword(new EncryptModel
            {
                Data = dto.Password,
                HashKey = dbUser.HashKey
            });

            for (int i = 0; i < encryptedData.HashedData.Length; i++)
            {
                if (encryptedData.HashedData[i] != dbUser.Password[i])
                {
                    throw new Exception("Invalid password");
                }
            }

            var token = await _tokenService.GenerateToken(dbUser);
            return new LoginResponseDto
            {
                Username = dbUser.Email,
                Token = token,
            };
        }
    }
}
