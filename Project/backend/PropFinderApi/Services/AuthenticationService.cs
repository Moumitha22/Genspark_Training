using PropFinderApi.Interfaces;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Exceptions;
using PropFinderApi.Mappers;

namespace PropFinderApi.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService _userService;
        private readonly IEncryptionService _encryptionService;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly UserMapper _userMapper;

        public AuthenticationService(
            IUserService userService,
            IEncryptionService encryptionService,
            ITokenService tokenService,
            IRefreshTokenService refreshTokenService)
        {
            _userService = userService;
            _encryptionService = encryptionService;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
            _userMapper = new UserMapper();
        }

        public async Task<string> RegisterAsync(UserRegisterRequestDto registerRequest)
        {
            var existingUser = await _userService.GetUserByEmailAsync(registerRequest.Email);
            if (existingUser != null)
            {
                throw new BadRequestException("User with this email already exists.");
            }

            var hashedPassword = _encryptionService.HashPassword(registerRequest.Password);

            var newUser = _userMapper.MapRegisterRequestDtoToUser(registerRequest, hashedPassword);

            await _userService.CreateUserAsync(newUser);

            return "User registered successfully";
        }

        public async Task<UserLoginResponseDto> LoginAsync(UserLoginRequestDto loginRequest)
        {
            var user = await _userService.GetUserByEmailAsync(loginRequest.Email.Trim());
            if (user == null || !_encryptionService.VerifyPassword(loginRequest.Password, user.PasswordHash))
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            var accessToken = await _tokenService.GenerateAccessTokenAsync(user);
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);

            return _userMapper.MapUserToLoginResponseDto(user, accessToken, refreshToken);
        }

        public async Task LogoutAsync(string refreshToken)
        {
            await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken);
        }

        public async Task<UserLoginResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var isValid = await _refreshTokenService.IsRefreshTokenValidAsync(refreshToken);
            if (!isValid)
            {
                throw new BadRequestException("Invalid refresh token");
            }

            var user = await _refreshTokenService.GetUserByRefreshTokenAsync(refreshToken);
            if (user == null)
            {
                throw new NotFoundException("User not found for this refresh token");
            }

            var accessToken = await _tokenService.GenerateAccessTokenAsync(user);
            
            var newRefreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);
            await _refreshTokenService.RevokeAndReplaceAsync(refreshToken, newRefreshToken);

            return _userMapper.MapUserToLoginResponseDto(user, accessToken, newRefreshToken);
        }
    }
}
