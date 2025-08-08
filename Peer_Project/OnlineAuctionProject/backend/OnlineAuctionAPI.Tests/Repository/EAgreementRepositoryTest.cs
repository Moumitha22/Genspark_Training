using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Repositories;

namespace OnlineAuctionAPI.Tests.Repository
{
    public class EAgreementRepositoryTests
    {
        private AuctionContext _context;
        private EAgreementRepository _repo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AuctionContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AuctionContext(options);
            _repo = new EAgreementRepository(_context);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsAgreement()
        {
            var agreement = new EAgreement
            {
                Id = Guid.NewGuid(),
                AuctionItemId = Guid.NewGuid(),
                BiddingId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                AuctionItem = new AuctionItem(),
                Bidding = new BidItem(),
            };
            await _context.EAgreements.AddAsync(agreement);
            await _context.SaveChangesAsync();

            var result = await _repo.GetByIdAsync(agreement.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(agreement.Id, result.Id);
        }

        [Test]
        public async Task GetAllWithBidItemsAsync_ReturnsAgreements()
        {
            var agreement = new EAgreement
            {
                Id = Guid.NewGuid(),
                AuctionItemId = Guid.NewGuid(),
                BiddingId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                AuctionItem = new AuctionItem(),
                Bidding = new BidItem(),
            };
            await _context.EAgreements.AddAsync(agreement);
            await _context.SaveChangesAsync();

            var result = await _repo.GetAllWithBidItemsAsync();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
        }

        [Test]
        public async Task GetByBiddingIdAsync_ReturnsAgreements()
        {
            var biddingId = Guid.NewGuid();
            var agreement = new EAgreement
            {
                Id = Guid.NewGuid(),
                AuctionItemId = Guid.NewGuid(),
                BiddingId = biddingId,
                CreatedAt = DateTime.UtcNow,
                AuctionItem = new AuctionItem(),
            };
            await _context.EAgreements.AddAsync(agreement);
            await _context.SaveChangesAsync();

            var result = await _repo.GetByBiddingIdAsync(biddingId);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(biddingId, result.First().BiddingId);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
