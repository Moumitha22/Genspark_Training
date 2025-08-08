using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Exceptions;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;
using OnlineAuctionAPI.Repositories;
using OnlineAuctionAPI.Service;

namespace OnlineAuctionAPI.Tests.Service
{
    public class AuthenticationServiceTests
    {
        private AuctionContext _dbContext;
        private IUserRepository _userRepo;
        private Mock<ITokenService> _mockTokenService;
        private Mock<IPasswordService> _mockPasswordService;
        private AuthenticationService _authService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AuctionContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new AuctionContext(options);

            _mockTokenService = new Mock<ITokenService>();
            _mockPasswordService = new Mock<IPasswordService>();
            _userRepo = new UserRepository(_dbContext);

            _authService = new AuthenticationService(
                _mockTokenService.Object,
                _mockPasswordService.Object,
                _userRepo
            );
        }

        [Test]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                Username = "testuser",
                Role = UserRole.Bidder,
                Password = "hashed123",
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var loginDto = new UserLoginRequestDto { Email = user.Email, Password = "plain123" };

            _mockPasswordService
                .Setup(p => p.VerifyPassword(user.Password, loginDto.Password))
                .Returns(true);
            _mockTokenService
                .Setup(t => t.GenerateTokensAsync(user))
                .ReturnsAsync(
                    new TokenDto { AccessToken = "access123", RefreshToken = "refresh123" }
                );

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Email, result.Email);
            Assert.AreEqual("access123", result.Token);
        }

        [Test]
        public void LoginAsync_InvalidPassword_ThrowsException()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "fail@example.com",
                Username = "wrongpass",
                Role = UserRole.Seller,
                Password = "hashedFail",
            };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            var loginDto = new UserLoginRequestDto { Email = user.Email, Password = "wrong123" };

            _mockPasswordService
                .Setup(p => p.VerifyPassword(user.Password, loginDto.Password))
                .Returns(false);

            // Act + Assert
            Assert.ThrowsAsync<InvalidException>(async () =>
                await _authService.LoginAsync(loginDto)
            );
        }

        [Test]
        public async Task LogoutAsync_ValidToken_RevokesSuccessfully()
        {
            // Arrange
            var token = new RefreshToken { Token = "validToken", IsRevoked = false };
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "logout@example.com",
                Username = "logoutuser",
                Role = UserRole.Bidder,
                Password = "pwd",
                RefreshTokens = new List<RefreshToken> { token },
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            await _authService.LogoutAsync("validToken");

            // Assert
            var updated = _dbContext.Users.First(u => u.Email == "logout@example.com");
            Assert.IsTrue(updated.RefreshTokens.First().IsRevoked);
            Assert.IsNotNull(updated.RefreshTokens.First().RevokedAt);
        }

        [Test]
        public void LogoutAsync_InvalidToken_ThrowsException()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "failLogout@example.com",
                Password = "pwd",
                RefreshTokens = new List<RefreshToken>
                {
                    new RefreshToken { Token = "valid123", IsRevoked = true },
                },
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            // Act + Assert
            Assert.ThrowsAsync<InvalidException>(async () =>
                await _authService.LogoutAsync("wrongToken")
            );
        }

        [TearDown]
        public void Cleanup()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
