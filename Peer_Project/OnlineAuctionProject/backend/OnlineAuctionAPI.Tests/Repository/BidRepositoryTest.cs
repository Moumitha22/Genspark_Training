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
    public class BidItemRepositoryTests
    {
        private AuctionContext _context;
        private BidItemRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AuctionContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AuctionContext(options);
            _context.Database.EnsureCreated();

            _repository = new BidItemRepository(_context);
        }

        [TearDown]
        public void Teardown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetBidsByAuctionAsync_ReturnsSortedBids()
        {
            var auctionId = Guid.NewGuid();
            var bidder = new User
            {
                Id = Guid.NewGuid(),
                Username = "UserA",
                Password = "password",
            };
            var bid1 = new BidItem
            {
                Id = Guid.NewGuid(),
                AuctionItemId = auctionId,
                Amount = 100,
                Bidder = bidder,
            };
            var bid2 = new BidItem
            {
                Id = Guid.NewGuid(),
                AuctionItemId = auctionId,
                Amount = 200,
                Bidder = bidder,
            };

            _context.Users.Add(bidder);
            _context.BidItems.AddRange(bid1, bid2);
            await _context.SaveChangesAsync();

            var result = await _repository.GetBidsByAuctionAsync(auctionId);

            Assert.That(result, Is.Not.Null.And.Count.EqualTo(2));
            Assert.That(result.First().Amount, Is.EqualTo(200));
        }

        [Test]
        public async Task GetHighestBidAsync_ReturnsTopBid()
        {
            var auctionId = Guid.NewGuid();
            _context.BidItems.AddRange(
                new BidItem
                {
                    Id = Guid.NewGuid(),
                    AuctionItemId = auctionId,
                    Amount = 150,
                    BidTime = DateTime.UtcNow,
                },
                new BidItem
                {
                    Id = Guid.NewGuid(),
                    AuctionItemId = auctionId,
                    Amount = 250,
                    BidTime = DateTime.UtcNow.AddMinutes(-1),
                }
            );
            await _context.SaveChangesAsync();

            var result = await _repository.GetHighestBidAsync(auctionId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Amount, Is.EqualTo(250));
        }

        [Test]
        public async Task GetBidsByUserIdAsync_ReturnsUserBids()
        {
            var userId = Guid.NewGuid();
            var auctionId = Guid.NewGuid();

            _context.BidItems.AddRange(
                new BidItem
                {
                    Id = Guid.NewGuid(),
                    AuctionItemId = auctionId,
                    Amount = 100,
                    BidderId = userId,
                }
            );
            await _context.SaveChangesAsync();

            var result = await _repository.GetBidsByUserIdAsync(userId);

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetBidsByAuctionAsync_Exception_ThrowsRepositoryOperationException()
        {
            var mockRepo = new BidItemRepository(null);

            Assert.ThrowsAsync<RepositoryOperationException>(async () =>
            {
                await mockRepo.GetBidsByAuctionAsync(Guid.NewGuid());
            });
        }

        [Test]
        public void GetHighestBidAsync_Exception_ThrowsRepositoryOperationException()
        {
            var mockRepo = new BidItemRepository(null);

            Assert.ThrowsAsync<RepositoryOperationException>(async () =>
            {
                await mockRepo.GetHighestBidAsync(Guid.NewGuid());
            });
        }
    }
}
