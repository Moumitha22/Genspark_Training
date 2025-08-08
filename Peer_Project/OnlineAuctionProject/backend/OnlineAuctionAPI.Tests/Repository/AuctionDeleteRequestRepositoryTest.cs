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
    public class AuctionDeleteRequestRepositoryTests
    {
        private AuctionContext _context;
        private AuctionDeleteRequestRepository _repo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AuctionContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AuctionContext(options);
            _repo = new AuctionDeleteRequestRepository(_context);
        }

        [Test]
        public async Task AuctionDeleteRequestAsync_CreatesRequest()
        {
            var auctionItem = new AuctionItem { Id = Guid.NewGuid(), Name = "Test Item" };
            await _context.AuctionItems.AddAsync(auctionItem);
            await _context.SaveChangesAsync();

            var dto = new AuctionDeleteRequestDto
            {
                AuctionItemId = auctionItem.Id,
                Reason = "Test Reason",
                UserId = Guid.NewGuid(),
            };

            await _repo.AuctionDeleteRequestAsync(dto);

            var request = await _context.AuctionDeleteRequests.FirstOrDefaultAsync(r =>
                r.AuctionItemId == auctionItem.Id
            );
            Assert.IsNotNull(request);
            Assert.AreEqual("Test Reason", request.Reason);
        }

        [Test]
        public void AuctionDeleteRequestAsync_NullDto_ThrowsRepositoryOperationException()
        {
            Assert.ThrowsAsync<RepositoryOperationException>(async () =>
                await _repo.AuctionDeleteRequestAsync(null)
            );
        }

        [Test]
        public void AuctionDeleteRequestAsync_AuctionItemNotFound_ThrowsRepositoryOperationException()
        {
            var dto = new AuctionDeleteRequestDto
            {
                AuctionItemId = Guid.NewGuid(),
                Reason = "Test Reason",
                UserId = Guid.NewGuid(),
            };

            Assert.ThrowsAsync<RepositoryOperationException>(async () =>
                await _repo.AuctionDeleteRequestAsync(dto)
            );
        }

        [Test]
        public async Task RejectAuctionDeleteRequestAsync_RejectsRequest()
        {
            var auctionItem = new AuctionItem { Id = Guid.NewGuid(), Name = "Test Item" };
            await _context.AuctionItems.AddAsync(auctionItem);
            var request = new AuctionDeleteRequest
            {
                Id = Guid.NewGuid(),
                AuctionItemId = auctionItem.Id,
                Reason = "Test Reason",
                UserId = Guid.NewGuid(),
                IsApproved = true,
            };
            await _context.AuctionDeleteRequests.AddAsync(request);
            await _context.SaveChangesAsync();

            var rejectDto = new AuctionDeleteRequestRejectDto
            {
                AuctionItemId = auctionItem.Id,
                Remarks = "Rejected",
            };

            await _repo.RejectAuctionDeleteRequestAsync(rejectDto);

            var updatedRequest = await _context.AuctionDeleteRequests.FirstOrDefaultAsync(r =>
                r.AuctionItemId == auctionItem.Id
            );
            Assert.IsFalse(updatedRequest.IsApproved);
            Assert.AreEqual("Rejected", updatedRequest.Remarks);
        }

        [Test]
        public void RejectAuctionDeleteRequestAsync_NullDto_ThrowsRepositoryOperationException()
        {
            Assert.ThrowsAsync<RepositoryOperationException>(async () =>
                await _repo.RejectAuctionDeleteRequestAsync(null)
            );
        }

        [Test]
        public void RejectAuctionDeleteRequestAsync_RequestNotFound_ThrowsRepositoryOperationException()
        {
            var rejectDto = new AuctionDeleteRequestRejectDto
            {
                AuctionItemId = Guid.NewGuid(),
                Remarks = "Rejected",
            };

            Assert.ThrowsAsync<RepositoryOperationException>(async () =>
                await _repo.RejectAuctionDeleteRequestAsync(rejectDto)
            );
        }

        [Test]
        public async Task GetAuctionDeleteRequestByAuctionIdAsync_ReturnsRequest()
        {
            var auctionItem = new AuctionItem { Id = Guid.NewGuid(), Name = "Test Item" };
            await _context.AuctionItems.AddAsync(auctionItem);
            var request = new AuctionDeleteRequest
            {
                Id = Guid.NewGuid(),
                AuctionItemId = auctionItem.Id,
                Reason = "Test Reason",
                UserId = Guid.NewGuid(),
                IsApproved = false,
            };
            await _context.AuctionDeleteRequests.AddAsync(request);
            await _context.SaveChangesAsync();

            var result = await _repo.GetAuctionDeleteRequestByAuctionIdAsync(auctionItem.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(request.Id, result.Id);
        }

        [Test]
        public void GetAuctionDeleteRequestByAuctionIdAsync_EmptyId_ThrowsRepositoryOperationException()
        {
            Assert.ThrowsAsync<RepositoryOperationException>(async () =>
                await _repo.GetAuctionDeleteRequestByAuctionIdAsync(Guid.Empty)
            );
        }

        [Test]
        public void GetAuctionDeleteRequestByAuctionIdAsync_NotFound_ThrowsRepositoryOperationException()
        {
            Assert.ThrowsAsync<RepositoryOperationException>(async () =>
                await _repo.GetAuctionDeleteRequestByAuctionIdAsync(Guid.NewGuid())
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
