using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NUnit.Framework;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Exceptions;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;
using OnlineAuctionAPI.Repositories;

namespace OnlineAuctionAPI.Tests.Repository
{
    public class VirtualWalletRepositoryTests
    {
        private AuctionContext _context;
        private VirtualWalletRepository _repo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AuctionContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            _context = new AuctionContext(options);
            _repo = new VirtualWalletRepository(_context);
        }

        [Test]
        public async Task AddVirtualWalletAsync_AddsWalletAndHistory()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                Password = "pwd",
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var dto = new VirtualWalletAddDto { Balance = 500 };
            await _repo.AddVirtualWalletAsync(user.Id, dto);

            var wallet = await _context.VirtualWallets.FirstOrDefaultAsync(w =>
                w.UserId == user.Id
            );
            Assert.IsNotNull(wallet);
            Assert.AreEqual(500, wallet.Balance);

            var history = await _context.VirtualWalletHistories.FirstOrDefaultAsync(h =>
                h.VirtualWalletId == wallet.Id
            );
            Assert.IsNotNull(history);
            Assert.AreEqual(500, history.Amount);
        }

        [Test]
        public async Task AddFundsToWalletAndHistoryAsync_AddsFundsAndHistory()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "funds@example.com",
                Password = "pwd",
            };
            var wallet = new VirtualWallet
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Balance = 100,
            };
            user.VirtualWallet = wallet;
            await _context.Users.AddAsync(user);
            await _context.VirtualWallets.AddAsync(wallet);
            await _context.SaveChangesAsync();

            await _repo.AddFundsToWalletAndHistoryAsync(user.Id, 200);

            var updatedWallet = await _context.VirtualWallets.FindAsync(wallet.Id);
            Assert.AreEqual(300, updatedWallet.Balance);

            var history = await _context.VirtualWalletHistories.FirstOrDefaultAsync(h =>
                h.VirtualWalletId == wallet.Id && h.Amount == 200
            );
            Assert.IsNotNull(history);
        }

        [Test]
        public void AddFundsToWalletAndHistoryAsync_UserNotFound_ThrowsRepositoryOperationException()
        {
            var userId = Guid.NewGuid();
            Assert.ThrowsAsync<RepositoryOperationException>(async () =>
                await _repo.AddFundsToWalletAndHistoryAsync(userId, 100)
            );
        }

        [Test]
        public async Task AddHistoryAsync_AddsHistory()
        {
            var wallet = new VirtualWallet
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Balance = 100,
            };
            await _context.VirtualWallets.AddAsync(wallet);
            await _context.SaveChangesAsync();

            var history = new VirtualWalletHistory
            {
                Id = Guid.NewGuid(),
                VirtualWalletId = wallet.Id,
                Amount = 50,
                TransactionDate = DateTime.UtcNow,
            };

            await _repo.AddHistoryAsync(history);

            var savedHistory = await _context.VirtualWalletHistories.FindAsync(history.Id);
            Assert.IsNotNull(savedHistory);
            Assert.AreEqual(50, savedHistory.Amount);
        }

        [Test]
        public async Task GetByUserIdAsync_ReturnsWallet()
        {
            var wallet = new VirtualWallet
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Balance = 100,
            };
            await _context.VirtualWallets.AddAsync(wallet);
            await _context.SaveChangesAsync();

            var result = await _repo.GetByUserIdAsync(wallet.UserId);

            Assert.IsNotNull(result);
            Assert.AreEqual(wallet.Id, result.Id);
        }

        [Test]
        public void GetByUserIdAsync_UserNotFound_ReturnsNull()
        {
            var userId = Guid.NewGuid();
            var result = _repo.GetByUserIdAsync(userId).Result;
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetVirtualWalletHistoryByUserIdAsync_ReturnsHistory()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "history@example.com",
                Password = "pwd",
            };
            var wallet = new VirtualWallet
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Balance = 100,
            };
            user.VirtualWallet = wallet;
            await _context.Users.AddAsync(user);
            await _context.VirtualWallets.AddAsync(wallet);

            var history = new VirtualWalletHistory
            {
                Id = Guid.NewGuid(),
                VirtualWalletId = wallet.Id,
                Amount = 25,
                TransactionDate = DateTime.UtcNow,
            };
            await _context.VirtualWalletHistories.AddAsync(history);
            await _context.SaveChangesAsync();

            var result = await _repo.GetVirtualWalletHistoryByUserIdAsync(user.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(25, result[0].Amount);
        }

        [Test]
        public void GetVirtualWalletHistoryByUserIdAsync_UserNotFound_ThrowsRepositoryOperationException()
        {
            var userId = Guid.NewGuid();
            Assert.ThrowsAsync<RepositoryOperationException>(async () =>
                await _repo.GetVirtualWalletHistoryByUserIdAsync(userId)
            );
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
