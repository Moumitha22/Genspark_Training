using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;
using OnlineAuctionAPI.Service;

[TestFixture]
public class TokenServiceTests
{
    private AuctionContext _context;
    private TokenService _tokenService;
    private Mock<IUserRepository> _mockUserRepository;
    private IConfiguration _configuration;
    private User _testUser;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AuctionContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AuctionContext(options);

        var inMemorySettings = new Dictionary<string, string>
        {
            { "Keys:JwtTokenKey", "this_is_a_secure_jwt_token_key_123456789" },
            { "JwtSettings:AccessTokenExpiryMinutes", "15" },
            { "JwtSettings:RefreshTokenExpiryDays", "7" },
        };

        _configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

        _mockUserRepository = new Mock<IUserRepository>();
        _tokenService = new TokenService(_configuration, _mockUserRepository.Object);

        _testUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Role = UserRole.Bidder,
            Username = "TestUser",
            Password = "dummy-password",
        };
    }

    [Test]
    public async Task GenerateTokensAsync_ValidUser_ReturnsTokenDto()
    {
        // Arrange
        _mockUserRepository
            .Setup(x => x.AddRefreshTokenAsync(It.IsAny<RefreshToken>()))
            .Callback<RefreshToken>(async token =>
            {
                await _context.RefreshTokens.AddAsync(token);
                await _context.SaveChangesAsync();
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _tokenService.GenerateTokensAsync(_testUser);

        // Assert
        Assert.IsNotNull(result.AccessToken);
        Assert.IsNotNull(result.RefreshToken);
        Assert.IsTrue(await _context.RefreshTokens.AnyAsync());
    }

    [Test]
    public async Task RefreshTokenAsync_ValidToken_ReturnsNewTokens()
    {
        // Arrange
        RefreshToken savedToken = null;

        _mockUserRepository
            .Setup(x => x.AddRefreshTokenAsync(It.IsAny<RefreshToken>()))
            .Callback<RefreshToken>(token => savedToken = token)
            .Returns(Task.CompletedTask);

        _mockUserRepository
            .Setup(x => x.GetRefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(
                (string token) =>
                    savedToken != null && savedToken.Token == token ? savedToken : null
            );

        _mockUserRepository.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync(_testUser);

        var tokenResult = await _tokenService.GenerateTokensAsync(_testUser);

        // Act
        var refreshed = await _tokenService.RefreshTokenAsync(tokenResult.RefreshToken);

        // Assert
        Assert.IsNotNull(refreshed.Token);
        Assert.IsNotNull(refreshed.RefreshToken);
        Assert.AreNotEqual(tokenResult.RefreshToken, refreshed.RefreshToken);
    }

    [Test]
    public void RefreshTokenAsync_InvalidToken_ThrowsUnauthorized()
    {
        // Arrange
        string invalidToken = "invalid_token";
        _mockUserRepository
            .Setup(x => x.GetRefreshTokenAsync(invalidToken))
            .ReturnsAsync((RefreshToken)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _tokenService.RefreshTokenAsync(invalidToken)
        );
        Assert.That(ex.Message, Is.EqualTo("Invalid, expired, or revoked refresh token"));
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
