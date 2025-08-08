using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models.DTOs;
using ChienVHShopOnline.Mappers;
using AutoMapper;
using ChienVHShopOnline.Models;

namespace ChienVHShopOnline.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService _userService;
        private readonly IEncryptionService _encryptionService;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IMapper _mapper;

        public AuthenticationService(
            IUserService userService,
            IEncryptionService encryptionService,
            ITokenService tokenService,
            IRefreshTokenService refreshTokenService,
            IMapper mapper)
        {
            _userService = userService;
            _encryptionService = encryptionService;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
            _mapper = mapper;
        }

        public async Task<string> RegisterAsync(UserRegisterRequestDto registerRequest)
        {
            var existingUser = await _userService.GetUserByEmailAsync(registerRequest.Email);

            if (existingUser != null)
                throw new InvalidOperationException("An account with this email already exists.");

            var hashedPassword = _encryptionService.HashPassword(registerRequest.Password);

            var newUser = _mapper.Map<User>(registerRequest);
            newUser.PasswordHash = hashedPassword;

            await _userService.CreateUser(newUser);

            return "User registered successfully";
        }

        public async Task<UserLoginResponseDto> LoginAsync(UserLoginRequestDto loginRequest)
        {
            var user = await _userService.GetUserByEmailAsync(loginRequest.Email);

            if (user == null || !_encryptionService.VerifyPassword(loginRequest.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            var accessToken = await _tokenService.GenerateAccessTokenAsync(user);
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);

            var loginResponse = _mapper.Map<UserLoginResponseDto>(user);
            loginResponse.AccessToken = accessToken;
            loginResponse.RefreshToken = refreshToken;

            return loginResponse;
        }

        public async Task LogoutAsync(string refreshToken)
        {
            await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken);
        }

        public async Task<UserLoginResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var isValid = await _refreshTokenService.IsRefreshTokenValidAsync(refreshToken);
            if (!isValid)
                throw new ArgumentException("Invalid refresh token");

            var user = await _refreshTokenService.GetUserByRefreshTokenAsync(refreshToken);
            if (user == null)
                throw new InvalidOperationException("User not found for this refresh token");

            var accessToken = await _tokenService.GenerateAccessTokenAsync(user);
            var newRefreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);
            await _refreshTokenService.RevokeAndReplaceAsync(refreshToken, newRefreshToken);

            var response = _mapper.Map<UserLoginResponseDto>(user);
            response.AccessToken = accessToken;
            response.RefreshToken = refreshToken;

            return response;
        }
    }
}
