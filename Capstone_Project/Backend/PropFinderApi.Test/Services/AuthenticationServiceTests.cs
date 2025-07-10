using Moq;
using NUnit.Framework;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Services;

namespace PropFinderApi.Tests.Services
{
    public class AuthenticationServiceTests
    {
        private Mock<IUserService> _userServiceMock;
        private Mock<IEncryptionService> _encryptionServiceMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<IRefreshTokenService> _refreshTokenServiceMock;
        private AuthenticationService _authService;

        [SetUp]
        public void SetUp()
        {
            _userServiceMock = new Mock<IUserService>();
            _encryptionServiceMock = new Mock<IEncryptionService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();

            _authService = new AuthenticationService(
                _userServiceMock.Object,
                _encryptionServiceMock.Object,
                _tokenServiceMock.Object,
                _refreshTokenServiceMock.Object
            );
        }

        [Test]
        public async Task RegisterAsync_WhenUserDoesNotExist_CreatesUser()
        {
            var dto = new UserRegisterRequestDto
            {
                Email = "new@example.com",
                Password = "password"
            };

            _userServiceMock.Setup(s => s.GetUserByEmailAsync(dto.Email)).ReturnsAsync((User?)null);
            _encryptionServiceMock.Setup(e => e.HashPassword(dto.Password)).Returns("hashed");

            var result = await _authService.RegisterAsync(dto);

            _userServiceMock.Verify(s => s.CreateUserAsync(It.Is<User>(u =>
                u.Email == dto.Email && u.PasswordHash == "hashed")), Times.Once);

            Assert.That(result, Is.EqualTo("User registered successfully"));
        }

        [Test]
        public void RegisterAsync_WhenUserExists_ThrowsBadRequest()
        {
            var dto = new UserRegisterRequestDto
            {
                Email = "existing@example.com",
                Password = "password"
            };

            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = "hashed"
            };

            _userServiceMock.Setup(s => s.GetUserByEmailAsync(dto.Email)).ReturnsAsync(existingUser);

            Assert.ThrowsAsync<BadRequestException>(() => _authService.RegisterAsync(dto));
        }


        [Test]
        public async Task LoginAsync_WithValidCredentials_ReturnsTokens()
        {
            var dto = new UserLoginRequestDto
            {
                Email = "user@example.com",
                Password = "plain"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = "hashed"
            };

            _userServiceMock.Setup(s => s.GetUserByEmailAsync(dto.Email)).ReturnsAsync(user);
            _encryptionServiceMock.Setup(e => e.VerifyPassword(dto.Password, "hashed")).Returns(true);
            _tokenServiceMock.Setup(t => t.GenerateAccessTokenAsync(user)).ReturnsAsync("access");
            _refreshTokenServiceMock.Setup(r => r.GenerateRefreshTokenAsync(user.Id)).ReturnsAsync("refresh");

            var result = await _authService.LoginAsync(dto);

            Assert.That(result.AccessToken, Is.EqualTo("access"));
            Assert.That(result.RefreshToken, Is.EqualTo("refresh"));
            Assert.That(result.Username, Is.EqualTo(user.Email));
        }

        [Test]
        public void LoginAsync_WithInvalidPassword_ThrowsUnauthorized()
        {
            var dto = new UserLoginRequestDto
            {
                Email = "user@example.com",
                Password = "wrong"
            };
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = "hashed"
            };

            _userServiceMock.Setup(s => s.GetUserByEmailAsync(dto.Email)).ReturnsAsync(user);
            _encryptionServiceMock.Setup(e => e.VerifyPassword(dto.Password, "hashed")).Returns(false);

            Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(dto));
        }

        [Test]
        public void LoginAsync_UserNotFound_ThrowsUnauthorized()
        {
            var dto = new UserLoginRequestDto
            {
                Email = "user@example.com",
                Password = "any"
            };

            _userServiceMock.Setup(s => s.GetUserByEmailAsync(dto.Email)).ReturnsAsync((User?)null);

            Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(dto));
        }

        [Test]
        public async Task LogoutAsync_CallsRevoke()
        {
            var token = "some-refresh-token";

            await _authService.LogoutAsync(token);

            _refreshTokenServiceMock.Verify(r => r.RevokeRefreshTokenAsync(token), Times.Once);
        }

        [Test]
        public async Task RefreshTokenAsync_WithValidToken_ReturnsNewTokens()
        {
            var oldToken = "old-refresh";
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "refresh@example.com"
            };

            _refreshTokenServiceMock.Setup(r => r.IsRefreshTokenValidAsync(oldToken)).ReturnsAsync(true);
            _refreshTokenServiceMock.Setup(r => r.GetUserByRefreshTokenAsync(oldToken)).ReturnsAsync(user);
            _tokenServiceMock.Setup(t => t.GenerateAccessTokenAsync(user)).ReturnsAsync("new-access");
            _refreshTokenServiceMock.Setup(r => r.GenerateRefreshTokenAsync(user.Id)).ReturnsAsync("new-refresh");

            var result = await _authService.RefreshTokenAsync(oldToken);

            Assert.That(result.AccessToken, Is.EqualTo("new-access"));
            Assert.That(result.RefreshToken, Is.EqualTo("new-refresh"));
        }

        [Test]
        public void RefreshTokenAsync_InvalidToken_ThrowsBadRequest()
        {
            var token = "bad-token";
            _refreshTokenServiceMock.Setup(r => r.IsRefreshTokenValidAsync(token)).ReturnsAsync(false);

            Assert.ThrowsAsync<BadRequestException>(() => _authService.RefreshTokenAsync(token));
        }

        [Test]
        public void RefreshTokenAsync_UserNotFound_ThrowsNotFound()
        {
            var token = "valid-but-no-user";
            _refreshTokenServiceMock.Setup(r => r.IsRefreshTokenValidAsync(token)).ReturnsAsync(true);
            _refreshTokenServiceMock.Setup(r => r.GetUserByRefreshTokenAsync(token)).ReturnsAsync((User?)null);

            Assert.ThrowsAsync<NotFoundException>(() => _authService.RefreshTokenAsync(token));
        }

    }
}