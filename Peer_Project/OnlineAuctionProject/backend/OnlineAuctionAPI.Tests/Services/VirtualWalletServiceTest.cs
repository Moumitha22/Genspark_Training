using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;
using OnlineAuctionAPI.Services;

namespace OnlineAuctionAPI.Tests.Service
{
    public class VirtualWalletServiceTests
    {
        private Mock<IVirtualWalletRepository> _mockWalletRepo;
        private Mock<IUserRepository> _mockUserRepo;
        private VirtualWalletService _service;

        [SetUp]
        public void Setup()
        {
            _mockWalletRepo = new Mock<IVirtualWalletRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _service = new VirtualWalletService(_mockWalletRepo.Object, _mockUserRepo.Object);
        }

        [Test]
        public async Task AddFundsToVirtualWalletAsync_CallsRepositoryAndReturnsUser()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };
            _mockWalletRepo
                .Setup(r => r.AddFundsToWalletAndHistoryAsync(userId, 100))
                .Returns(Task.CompletedTask);
            _mockUserRepo.Setup(r => r.GetByIdWithVirtualWalletAsync(userId)).ReturnsAsync(user);

            var result = await _service.AddFundsToVirtualWalletAsync(userId, 100);

            Assert.AreEqual(user, result);
        }

        [Test]
        public void AddVirtualWalletToUserAsync_UserNotFound_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            var dto = new VirtualWalletAddDto { Balance = 100 };
            _mockUserRepo
                .Setup(r => r.GetByIdWithVirtualWalletAsync(userId))
                .ReturnsAsync((User)null);

            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _service.AddVirtualWalletToUserAsync(userId, dto)
            );
        }

        [Test]
        public void AddVirtualWalletToUserAsync_UserAlreadyHasWallet_ThrowsInvalidOperationException()
        {
            var userId = Guid.NewGuid();
            var dto = new VirtualWalletAddDto { Balance = 100 };
            var user = new User
            {
                Id = userId,
                VirtualWallet = new VirtualWallet { Balance = 50 },
            };
            _mockUserRepo.Setup(r => r.GetByIdWithVirtualWalletAsync(userId)).ReturnsAsync(user);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.AddVirtualWalletToUserAsync(userId, dto)
            );
        }

        [Test]
        public void AddVirtualWalletToUserAsync_ExceedsLimit_ThrowsInvalidDataException()
        {
            var userId = Guid.NewGuid();
            var dto = new VirtualWalletAddDto { Balance = 5_000_001 };
            var user = new User { Id = userId, VirtualWallet = null };
            _mockUserRepo.Setup(r => r.GetByIdWithVirtualWalletAsync(userId)).ReturnsAsync(user);

            Assert.ThrowsAsync<InvalidDataException>(async () =>
                await _service.AddVirtualWalletToUserAsync(userId, dto)
            );
        }

        [Test]
        public void AddVirtualWalletToUserAsync_NonPositiveBalance_ThrowsInvalidDataException()
        {
            var userId = Guid.NewGuid();
            var dto = new VirtualWalletAddDto { Balance = 0 };
            var user = new User { Id = userId, VirtualWallet = null };
            _mockUserRepo.Setup(r => r.GetByIdWithVirtualWalletAsync(userId)).ReturnsAsync(user);

            Assert.ThrowsAsync<InvalidDataException>(async () =>
                await _service.AddVirtualWalletToUserAsync(userId, dto)
            );
        }

        [Test]
        public async Task AddVirtualWalletToUserAsync_Valid_AddsWalletAndReturnsUser()
        {
            var userId = Guid.NewGuid();
            var dto = new VirtualWalletAddDto { Balance = 100 };
            var user = new User { Id = userId, VirtualWallet = null };
            var updatedUser = new User
            {
                Id = userId,
                VirtualWallet = new VirtualWallet { Balance = 100 },
            };

            _mockUserRepo
                .SetupSequence(r => r.GetByIdWithVirtualWalletAsync(userId))
                .ReturnsAsync(user)
                .ReturnsAsync(updatedUser);
            _mockWalletRepo
                .Setup(r => r.AddVirtualWalletAsync(userId, dto))
                .Returns(Task.CompletedTask);

            var result = await _service.AddVirtualWalletToUserAsync(userId, dto);

            Assert.AreEqual(updatedUser, result);
        }

        [Test]
        public void GetVirtualWalletByUserIdAsync_NotFound_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            _mockWalletRepo
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync((VirtualWallet)null);

            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _service.GetVirtualWalletByUserIdAsync(userId)
            );
        }

        [Test]
        public async Task GetVirtualWalletByUserIdAsync_ReturnsWallet()
        {
            var userId = Guid.NewGuid();
            var wallet = new VirtualWallet { UserId = userId, Balance = 100 };
            _mockWalletRepo.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(wallet);

            var result = await _service.GetVirtualWalletByUserIdAsync(userId);

            Assert.AreEqual(wallet, result);
        }

        [Test]
        public async Task GetVirtualWalletHistoryByUserIdAsync_ReturnsHistory()
        {
            var userId = Guid.NewGuid();
            var history = new List<VirtualWalletHistory> { new VirtualWalletHistory() };
            _mockWalletRepo
                .Setup(r => r.GetVirtualWalletHistoryByUserIdAsync(userId))
                .ReturnsAsync(history);

            var result = await _service.GetVirtualWalletHistoryByUserIdAsync(userId);

            Assert.AreEqual(history, result);
        }
    }
}
