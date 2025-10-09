using Moq;
using NUnit.Framework;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Services;
using PropFinderApi.Models.Enums;

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
        public async Task RegisterAsync_WhenUserDoesNotExistForRole_CreatesUser()
        {
            var dto = new UserRegisterRequestDto
            {
                Email = "newuser@example.com",
                Password = "securepassword",
                Role = UserRole.Lister
            };

            _userServiceMock.Setup(s => s.GetUserByEmailandRoleAsync(dto.Email, dto.Role)).ReturnsAsync((User?)null); 
            _encryptionServiceMock.Setup(e => e.HashPassword(dto.Password)).Returns("hashedPassword");

            var createdUser = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Role = dto.Role,
                PasswordHash = "hashedPassword"
            };

            _userServiceMock.Setup(s => s.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(createdUser);

            var result = await _authService.RegisterAsync(dto);

            Assert.That(result, Is.EqualTo("User registered successfully"));

            _userServiceMock.Verify(s => s.GetUserByEmailandRoleAsync(dto.Email, dto.Role), Times.Once);
            _encryptionServiceMock.Verify(e => e.HashPassword(dto.Password), Times.Once);
            _userServiceMock.Verify(s => s.CreateUserAsync(It.Is<User>(u =>
                u.Email == dto.Email &&
                u.Role == dto.Role &&
                u.PasswordHash == "hashedPassword"
            )), Times.Once);
        }

        [Test]
        public void RegisterAsync_WhenUserExistsForRole_ThrowsConflictException()
        {
            var dto = new UserRegisterRequestDto
            {
                Email = "existinguser@example.com",
                Password = "securepassword",
                Role = UserRole.Lister
            };

            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Role = dto.Role,
                PasswordHash = "existingHash"
            };

            _userServiceMock.Setup(s => s.GetUserByEmailandRoleAsync(dto.Email, dto.Role)).ReturnsAsync(existingUser);

            var ex = Assert.ThrowsAsync<ConflictException>(async () =>
                await _authService.RegisterAsync(dto));

            Assert.That(ex!.Message, Is.EqualTo("An account with this email and role already exists."));

            _userServiceMock.Verify(s => s.GetUserByEmailandRoleAsync(dto.Email, dto.Role), Times.Once);
            _encryptionServiceMock.Verify(e => e.HashPassword(It.IsAny<string>()), Times.Never);
            _userServiceMock.Verify(s => s.CreateUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task LoginAsync_WithValidCredentials_ReturnsTokens()
        {
            var loginRequestDto = new UserLoginRequestDto
            {
                Email = "validuser@example.com",
                Password = "securepassword",
                Role = UserRole.Lister
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = loginRequestDto.Email,
                Role = loginRequestDto.Role.Value,
                PasswordHash = "hashedPassword",
                IsDeleted = false
            };

            _userServiceMock
                .Setup(s => s.GetUserByEmailandRoleAsync(loginRequestDto.Email, loginRequestDto.Role.Value))
                .ReturnsAsync(user);

            _encryptionServiceMock
                .Setup(e => e.VerifyPassword(loginRequestDto.Password, user.PasswordHash))
                .Returns(true);

            _tokenServiceMock
                .Setup(t => t.GenerateAccessTokenAsync(user))
                .ReturnsAsync("access-token");

            _refreshTokenServiceMock
                .Setup(r => r.GenerateRefreshTokenAsync(user.Id))
                .ReturnsAsync("refresh-token");

            var result = await _authService.LoginAsync(loginRequestDto);

            Assert.That(result.AccessToken, Is.EqualTo("access-token"));
            Assert.That(result.RefreshToken, Is.EqualTo("refresh-token"));
            Assert.That(result.Username, Is.EqualTo(user.Email));
        }

        [Test]
        public void LoginAsync_WithInvalidPassword_ThrowsUnauthorizedException()
        {
            var loginRequest = new UserLoginRequestDto
            {
                Email = "validuser@example.com",
                Password = "wrongpassword",
                Role = UserRole.Lister
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = loginRequest.Email,
                Role = loginRequest.Role.Value,
                PasswordHash = "hashedPassword",
                IsDeleted = false
            };

            _userServiceMock
                .Setup(s => s.GetUserByEmailandRoleAsync(loginRequest.Email, loginRequest.Role.Value))
                .ReturnsAsync(user);

            _encryptionServiceMock
                .Setup(e => e.VerifyPassword(loginRequest.Password, user.PasswordHash))
                .Returns(false); 
                
            var ex = Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(loginRequest));
            Assert.That(ex!.Message, Is.EqualTo("Invalid email or password."));
        }


        [Test]
        public void LoginAsync_WhenUserNotFound_ThrowsUnauthorizedException()
        {
            var loginRequest = new UserLoginRequestDto
            {
                Email = "nonexistent@example.com",
                Password = "password",
                Role = UserRole.Lister
            };

            _userServiceMock
                .Setup(s => s.GetUserByEmailandRoleAsync(loginRequest.Email, loginRequest.Role.Value))
                .ReturnsAsync((User?)null);

            var ex = Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(loginRequest));
            Assert.That(ex!.Message, Is.EqualTo("Invalid email or password."));
        }

        [Test]
        public void LoginAsync_WhenUserIsDeleted_ThrowsNotFoundException()
        {
            var loginRequest = new UserLoginRequestDto
            {
                Email = "deleteduser@example.com",
                Password = "password",
                Role = UserRole.Lister
            };

            var deletedUser = new User
            {
                Id = Guid.NewGuid(),
                Email = loginRequest.Email,
                Role = loginRequest.Role.Value,
                PasswordHash = "hashedPassword",
                IsDeleted = true
            };

            _userServiceMock
                .Setup(s => s.GetUserByEmailandRoleAsync(loginRequest.Email, loginRequest.Role.Value))
                .ReturnsAsync(deletedUser);

            var ex = Assert.ThrowsAsync<NotFoundException>(() => _authService.LoginAsync(loginRequest));
            Assert.That(ex!.Message, Is.EqualTo("User has been disabled."));
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