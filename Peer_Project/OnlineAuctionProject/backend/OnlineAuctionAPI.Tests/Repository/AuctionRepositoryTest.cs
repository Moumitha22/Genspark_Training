using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Exceptions;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;
using OnlineAuctionAPI.Repositories;

namespace OnlineAuctionAPI.Tests
{
    public class AuctionRepositoryTests
    {
        private AuctionContext _context;
        private AuctionRepository _repo;

        [SetUp]
        public void Setup()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var options = new DbContextOptionsBuilder<AuctionContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AuctionContext(options);
            _context.Database.EnsureCreated();

            _repo = new AuctionRepository(_context);
        }

        [TearDown]
        public void Teardown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetActiveAuctionsAsync_ReturnsLiveAuctions()
        {
            var auction = new AuctionItem
            {
                Id = Guid.NewGuid(),
                Name = "Live Auction",
                Status = AuctionStatus.Live,
                StartTime = DateTime.UtcNow.AddMinutes(-10),
                EndTime = DateTime.UtcNow.AddMinutes(10),
            };
            _context.AuctionItems.Add(auction);
            await _context.SaveChangesAsync();

            var result = await _repo.GetActiveAuctionsAsync();

            Assert.IsNotEmpty(result);
            Assert.AreEqual(auction.Id, result.First().Id);
        }

        [Test]
        public async Task GetBidsAsync_WithBids_ReturnsAuctionWithBids()
        {
            var auctionId = Guid.NewGuid();
            var auction = new AuctionItem { Id = auctionId, Name = "Bid Auction" };
            var bidder = new User
            {
                Id = Guid.NewGuid(),
                Username = "Bidder1",
                Password = "dummy",
            };
            var bid = new BidItem
            {
                Id = Guid.NewGuid(),
                AuctionItemId = auctionId,
                Amount = 100,
                BidderId = bidder.Id,
                Bidder = bidder,
            };

            _context.Users.Add(bidder);
            _context.AuctionItems.Add(auction);
            _context.BidItems.Add(bid);
            await _context.SaveChangesAsync();

            var result = await _repo.GetBidsAsync(auctionId);
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Bids);
        }

        [Test]
        public async Task GetAllAuctionsWithFile_ReturnsAuctionsWithFiles()
        {
            var auction = new AuctionItem
            {
                Name = "Auction with File",
                FileAttachments = new List<FileData>
                {
                    new FileData
                    {
                        Id = 1,
                        Name = "doc.txt",
                        Data = new byte[] { 0x01 },
                    },
                },
                Seller = new User
                {
                    Id = Guid.NewGuid(),
                    Username = "Seller1",
                    Password = "dummy",
                },
            };
            _context.AuctionItems.Add(auction);
            await _context.SaveChangesAsync();

            var result = await _repo.GetAllAuctionsWithFile();
            Assert.IsNotEmpty(result);
            Assert.IsNotEmpty(result.First().FileAttachments);
            Assert.IsNotNull(result.First().Seller);
        }

        [Test]
        public async Task GetAuctionsWithFile_ReturnsSingleAuctionWithDetails()
        {
            var auctionId = Guid.NewGuid();
            var auction = new AuctionItem
            {
                Id = auctionId,
                FileAttachments = new List<FileData>
                {
                    new FileData
                    {
                        Id = 1,
                        Name = "img.jpg",
                        Data = new byte[] { 0x01 },
                    },
                },
                Seller = new User
                {
                    Id = Guid.NewGuid(),
                    Username = "Seller2",
                    Password = "dummy",
                },
            };
            _context.AuctionItems.Add(auction);
            await _context.SaveChangesAsync();

            var result = await _repo.GetAuctionsWithFile(auctionId);
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.FileAttachments);
            Assert.IsNotNull(result.Seller);
        }

        [Test]
        public async Task GetPagedItemsAsync_ReturnsPaginatedAuctions()
        {
            for (int i = 0; i < 15; i++)
            {
                _context.AuctionItems.Add(
                    new AuctionItem { Id = Guid.NewGuid(), Name = $"Auction {i}" }
                );
            }
            await _context.SaveChangesAsync();

            var (items, total) = await _repo.GetPagedItemsAsync(2, 5);
            Assert.AreEqual(5, items.Count());
            Assert.AreEqual(15, total);
        }

        [Test]
        public async Task UpdateWinningId_ValidUpdate_UpdatesWinner()
        {
            var auctionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var auction = new AuctionItem { Id = auctionId, EndTime = now.AddMinutes(10) };
            var bid = new BidItem
            {
                Id = Guid.NewGuid(),
                AuctionItemId = auctionId,
                Amount = 500,
                BidderId = userId,
                BidTime = now.AddMinutes(1),
            };
            var user = new User
            {
                Id = userId,
                Username = "TopBidder",
                Password = "dummy",
            };

            _context.Users.Add(user);
            _context.AuctionItems.Add(auction);
            _context.BidItems.Add(bid);
            await _context.SaveChangesAsync();

            var dto = new WinningIdUpdateDto { AuctionItemId = auctionId, WinningId = bid.Id };

            var result = await _repo.UpdateWinningId(dto);

            Assert.IsNotNull(result);
            Assert.AreEqual(bid.BidderId, result.WinnerId);
            Assert.AreEqual("TopBidder", result.WinnerName);
        }

        [Test]
        public void UpdateWinningId_NoAuction_ThrowsNotFound()
        {
            var dto = new WinningIdUpdateDto
            {
                AuctionItemId = Guid.NewGuid(),
                WinningId = Guid.NewGuid(),
            };

            var ex = Assert.ThrowsAsync<RepositoryOperationException>(() =>
                _repo.UpdateWinningId(dto)
            );
            Assert.IsInstanceOf<NotFoundException>(ex.InnerException);
        }

        [Test]
        public async Task UpdateWinningId_InvalidWinner_ThrowsNotFound()
        {
            var auctionId = Guid.NewGuid();
            var bidder = Guid.NewGuid();
            var wrongWinner = Guid.NewGuid();

            _context.AuctionItems.Add(new AuctionItem { Id = auctionId });
            _context.BidItems.Add(
                new BidItem
                {
                    AuctionItemId = auctionId,
                    Amount = 999,
                    BidderId = bidder,
                }
            );
            await _context.SaveChangesAsync();

            var dto = new WinningIdUpdateDto { AuctionItemId = auctionId, WinningId = wrongWinner };

            var ex = Assert.ThrowsAsync<RepositoryOperationException>(() =>
                _repo.UpdateWinningId(dto)
            );
            Assert.IsInstanceOf<NotFoundException>(ex.InnerException);
        }
    }
}
