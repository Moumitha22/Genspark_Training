using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Exceptions;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;
using OnlineAuctionAPI.Repositories;

namespace OnlineAuctionAPI.Tests.Repository
{
    public class UserAccountManageRepositoryTests
    {
        private AuctionContext _context;
        private UserAccountManageRepository _repo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AuctionContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AuctionContext(options);
            _repo = new UserAccountManageRepository(_context);
        }

        [Test]
        public async Task RestoreUserByEmailAsync_RestoresDeletedUser()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "restore@example.com",
                StatusId = 2,
                Password = "test@123",
            };
            await _context.Users.AddAsync(user);
            await _context.DeletedUsers.AddAsync(
                new DeletedUsers
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Reason = "Test",
                }
            );
            await _context.SaveChangesAsync();

            await _repo.RestoreUserByEmailAsync(user.Email);

            var restoredUser = await _context.Users.FindAsync(user.Id);
            Assert.AreEqual(1, restoredUser.StatusId);
            Assert.IsNull(
                await _context.DeletedUsers.FirstOrDefaultAsync(d => d.UserId == user.Id)
            );
        }

        [Test]
        public void RestoreUserByEmailAsync_UserNotFound_ThrowsRepositoryOperationException()
        {
            Assert.ThrowsAsync<RepositoryOperationException>(async () =>
                await _repo.RestoreUserByEmailAsync("notfound@example.com")
            );
        }

        [Test]
        public async Task GetDeleteReasonByEmailAsync_ReturnsReason()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "reason@example.com",
                StatusId = 2,
                Password = "TestPassword123",
            };
            await _context.Users.AddAsync(user);
            await _context.DeletedUsers.AddAsync(
                new DeletedUsers
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Reason = "ReasonTest",
                }
            );
            await _context.SaveChangesAsync();

            var reason = await _repo.GetDeleteReasonByEmailAsync(user.Email);

            Assert.AreEqual("ReasonTest", reason);
        }

        [Test]
        public async Task SoftDeleteUserAsync_DeletesUserAndAddsLog()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "delete@example.com",
                StatusId = 1,
                Password = "TestPassword124",
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var dto = new UserDeleteRequest { UserId = user.Id, Reason = "Soft delete" };
            await _repo.SoftDeleteUserAsync(dto);

            var deletedUser = await _context.Users.FindAsync(user.Id);
            Assert.AreEqual(2, deletedUser.StatusId);

            var deletedLog = await _context.DeletedUsers.FirstOrDefaultAsync(d =>
                d.UserId == user.Id
            );
            Assert.IsNotNull(deletedLog);
            Assert.AreEqual("Soft delete", deletedLog.Reason);
        }

        [Test]
        public async Task GetAllAsync_ReturnsDeletedUsers()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "all@example.com",
                StatusId = 2,
                Password = "TestPassword125",
            };
            await _context.Users.AddAsync(user);
            await _context.DeletedUsers.AddAsync(
                new DeletedUsers
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Reason = "TestAll",
                }
            );
            await _context.SaveChangesAsync();

            var result = await _repo.GetAllAsync();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("TestAll", result[0].Reason);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
