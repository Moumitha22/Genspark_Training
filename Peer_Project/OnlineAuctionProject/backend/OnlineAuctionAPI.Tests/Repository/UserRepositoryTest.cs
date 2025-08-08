using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Exceptions;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Repositories;

namespace OnlineAuctionAPI.Tests
{
    public class UserRepositoryTests
    {
        private AuctionContext _context;
        private UserRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AuctionContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AuctionContext(options);
            _context.Database.EnsureCreated();

            _repository = new UserRepository(_context);
        }

        [TearDown]
        public void Teardown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetByUsernameAsync_ValidUsername_ReturnsUser()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "john_doe",
                Email = "john@example.com",
                Password = "password123",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByUsernameAsync("john_doe");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo("john_doe"));
        }

        [Test]
        public void GetByUsernameAsync_NullUsername_ThrowsArgumentException()
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _repository.GetByUsernameAsync(null);
            });

            Assert.That(ex!.Message, Does.Contain("username"));
        }

        [Test]
        public async Task GetByEmailAsync_ValidEmail_ReturnsUser()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "jane",
                Email = "jane@example.com",
                Password = "password",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByEmailAsync("jane@example.com");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo("jane@example.com"));
        }

        [Test]
        public void GetByEmailAsync_EmptyEmail_ThrowsArgumentException()
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _repository.GetByEmailAsync("");
            });

            Assert.That(ex!.Message, Does.Contain("email"));
        }

        [Test]
        public async Task GetUserByRefreshTokenAsync_ValidToken_ReturnsUser()
        {
            var token = new RefreshToken { Id = Guid.NewGuid(), Token = "refresh123" };
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Password = "password123",
                RefreshTokens = new List<RefreshToken> { token },
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _repository.GetUserByRefreshTokenAsync("refresh123");

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Username, Is.EqualTo("testuser"));
        }

        [Test]
        public async Task GetRefreshTokenAsync_ValidToken_ReturnsToken()
        {
            var token = new RefreshToken { Id = Guid.NewGuid(), Token = "tokenABC" };
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();

            var result = await _repository.GetRefreshTokenAsync("tokenABC");

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Token, Is.EqualTo("tokenABC"));
        }

        [Test]
        public void GetByUsernameAsync_DbException_ThrowsRepositoryOperationException()
        {
            var brokenRepo = new UserRepository(null!);

            var ex = Assert.ThrowsAsync<RepositoryOperationException>(async () =>
            {
                await brokenRepo.GetByUsernameAsync("john");
            });

            Assert.That(ex!.Message, Does.Contain("Get users by userName"));
        }

        [Test]
        public void GetByEmailAsync_DbException_ThrowsRepositoryOperationException()
        {
            var brokenRepo = new UserRepository(null!);

            var ex = Assert.ThrowsAsync<RepositoryOperationException>(async () =>
            {
                await brokenRepo.GetByEmailAsync("jane@example.com");
            });

            Assert.That(ex!.Message, Does.Contain("Get users by Email"));
        }
    }
}
