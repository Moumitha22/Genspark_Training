using Moq;
using NUnit.Framework;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Services;
using System;

namespace PropFinderApi.Tests.Services
{
    public class RefreshTokenServiceTests
    {
        private Mock<IRefreshTokenRepository> _refreshTokenRepoMock;
        private Mock<IUserRepository> _userRepoMock;
        private RefreshTokenService _service;

        [SetUp]
        public void SetUp()
        {
            _refreshTokenRepoMock = new Mock<IRefreshTokenRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _service = new RefreshTokenService(_refreshTokenRepoMock.Object, _userRepoMock.Object);
        }

        [Test]
        public async Task GenerateRefreshTokenAsync_ReturnsTokenString_AndStoresToken()
        {
            var userId = Guid.NewGuid();

            RefreshToken? capturedToken = null;

            _refreshTokenRepoMock.Setup(r => r.Add(It.IsAny<RefreshToken>()))
                .Callback<RefreshToken>(t => capturedToken = t)
                .Returns<RefreshToken>(t => Task.FromResult(t));

            var result = await _service.GenerateRefreshTokenAsync(userId);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);
                Assert.That(result.Length, Is.GreaterThan(60));
                Assert.IsNotNull(capturedToken);
                Assert.That(capturedToken.UserId, Is.EqualTo(userId));
            });
        }

        [Test]
        public async Task IsRefreshTokenValidAsync_ReturnsTrue_IfValid()
        {
            var token = "valid-token";
            var validToken = new RefreshToken
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                RevokedAt = null
            };

            _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync(token)).ReturnsAsync(validToken);

            var result = await _service.IsRefreshTokenValidAsync(token);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task IsRefreshTokenValidAsync_ReturnsFalse_IfNull()
        {
            _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync("invalid")).ReturnsAsync((RefreshToken?)null);

            var result = await _service.IsRefreshTokenValidAsync("invalid");

            Assert.IsFalse(result);
        }

        [Test]
        public async Task IsRefreshTokenValidAsync_ReturnsFalse_IfExpired()
        {
            var token = new RefreshToken
            {
                ExpiresAt = DateTime.UtcNow.AddDays(-1),
                RevokedAt = null
            };

            _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync("expired")).ReturnsAsync(token);

            var result = await _service.IsRefreshTokenValidAsync("expired");

            Assert.IsFalse(result);
        }

        [Test]
        public async Task IsRefreshTokenValidAsync_ReturnsFalse_IfRevoked()
        {
            var token = new RefreshToken
            {
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                RevokedAt = DateTime.UtcNow
            };

            _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync("revoked")).ReturnsAsync(token);

            var result = await _service.IsRefreshTokenValidAsync("revoked");

            Assert.IsFalse(result);
        }

        [Test]
        public async Task RevokeRefreshTokenAsync_SetsRevokedAt()
        {
            var token = new RefreshToken { Id = Guid.NewGuid(), Token = "abc" };

            _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync("abc")).ReturnsAsync(token);

            await _service.RevokeRefreshTokenAsync("abc");

            _refreshTokenRepoMock.Verify(r => r.Update(token.Id, It.Is<RefreshToken>(t => t.RevokedAt != null)));
        }

        [Test]
        public void RevokeRefreshTokenAsync_ThrowsNotFound_WhenTokenMissing()
        {
            _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync("missing")).ReturnsAsync((RefreshToken?)null);

            Assert.ThrowsAsync<NotFoundException>(() => _service.RevokeRefreshTokenAsync("missing"));
        }

        [Test]
        public async Task RevokeAndReplaceAsync_RevokesAndLinksTokens()
        {
            var oldToken = new RefreshToken { Id = Guid.NewGuid(), Token = "old" };

            _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync("old")).ReturnsAsync(oldToken);

            await _service.RevokeAndReplaceAsync("old", "new");

            _refreshTokenRepoMock.Verify(r => r.Update(oldToken.Id, It.Is<RefreshToken>(t =>
                t.RevokedAt != null && t.ReplacedByToken == "new")));
        }

        [Test]
        public void RevokeAndReplaceAsync_ThrowsNotFound_WhenOldTokenMissing()
        {
            _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync("old")).ReturnsAsync((RefreshToken?)null);

            Assert.ThrowsAsync<NotFoundException>(() => _service.RevokeAndReplaceAsync("old", "new"));
        }

        [Test]
        public async Task GetUserByRefreshTokenAsync_ReturnsUser_IfTokenExists()
        {
            var userId = Guid.NewGuid();
            var token = new RefreshToken { UserId = userId };
            var user = new User { Id = userId };

            _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync("token")).ReturnsAsync(token);
            _userRepoMock.Setup(r => r.Get(userId)).ReturnsAsync(user);

            var result = await _service.GetUserByRefreshTokenAsync("token");

            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(userId));
        }

        [Test]
        public async Task GetUserByRefreshTokenAsync_ReturnsNull_IfTokenMissing()
        {
            _refreshTokenRepoMock.Setup(r => r.GetByTokenAsync("token")).ReturnsAsync((RefreshToken?)null);

            var result = await _service.GetUserByRefreshTokenAsync("token");

            Assert.IsNull(result);
        }
    }
}
