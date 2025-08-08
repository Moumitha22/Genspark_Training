using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Exceptions;
using OnlineAuctionAPI.Mapping;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;
using OnlineAuctionAPI.Repositories;
using OnlineAuctionAPI.Service;

namespace OnlineAuctionAPI.Tests
{
    public class BidItemServiceInMemoryTests
    {
        private AuctionContext _context;
        private BidItemRepository _bidItemRepo;
        private AuctionRepository _auctionItemRepo;
        private IMapper _mapper;
        private BidItemService _service;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        private Mock<IUserRepository> _mockUserRepo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AuctionContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AuctionContext(options);
            _context.Database.EnsureCreated();

            _bidItemRepo = new BidItemRepository(_context);
            _auctionItemRepo = new AuctionRepository(_context);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new BidProfile());
            });
            _mapper = mapperConfig.CreateMapper();

            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext)null);

            _mockUserRepo = new Mock<IUserRepository>();

            _service = new BidItemService(
                _bidItemRepo,
                _auctionItemRepo,
                _mockUserRepo.Object,
                _mapper,
                _mockHttpContextAccessor.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        public async Task PlaceBidAsync_ValidBid_Succeeds()
        {
            var auction = new AuctionItem
            {
                Id = Guid.NewGuid(),
                Name = "Test Auction",
                Status = AuctionStatus.Live,
                StartingPrice = 100,
                StartTime = DateTime.UtcNow.AddMinutes(-5),
                EndTime = DateTime.UtcNow.AddMinutes(10),
            };
            await _context.AuctionItems.AddAsync(auction);
            await _context.SaveChangesAsync();

            var bidderId = Guid.NewGuid();
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim("UserId", bidderId.ToString()),
            };
            var identity = new System.Security.Claims.ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new System.Security.Claims.ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            var virtualWallet = new VirtualWallet
            {
                Id = Guid.NewGuid(),
                UserId = bidderId,
                Balance = 1000,
            };
            var user = new User { Id = bidderId, VirtualWallet = virtualWallet };
            _mockUserRepo.Setup(x => x.GetByIdWithVirtualWalletAsync(bidderId)).ReturnsAsync(user);

            _service = new BidItemService(
                _bidItemRepo,
                _auctionItemRepo,
                _mockUserRepo.Object,
                _mapper,
                _mockHttpContextAccessor.Object
            );

            var bidDto = new BidItemAddDto
            {
                AuctionItemId = auction.Id,
                BidderId = bidderId,
                Amount = 150,
            };

            var result = await _service.PlaceBidAsync(bidDto);
            Assert.IsNotNull(result);
            Assert.AreEqual(bidDto.Amount, result.Amount);
        }

        [Test]
        public void PlaceBidAsync_InvalidAuction_Throws()
        {
            var bidDto = new BidItemAddDto
            {
                AuctionItemId = Guid.NewGuid(),
                BidderId = Guid.NewGuid(),
                Amount = 200,
            };

            Assert.ThrowsAsync<RepositoryOperationException>(() => _service.PlaceBidAsync(bidDto));
        }

        [Test]
        public async Task GetAllBidAsync_WithBids_ReturnsList()
        {
            var auction = new AuctionItem
            {
                Id = Guid.NewGuid(),
                Status = AuctionStatus.Live,
                StartingPrice = 100,
                StartTime = DateTime.UtcNow.AddMinutes(-10),
                EndTime = DateTime.UtcNow.AddMinutes(10),
            };
            var bid = new BidItem
            {
                Id = Guid.NewGuid(),
                AuctionItemId = auction.Id,
                Amount = 200,
                BidderId = Guid.NewGuid(),
            };

            await _context.AuctionItems.AddAsync(auction);
            await _context.BidItems.AddAsync(bid);
            await _context.SaveChangesAsync();

            var result = await _service.GetAllBidAsync();
            Assert.IsNotEmpty(result);
        }

        [Test]
        public async Task GetAllBidAsync_NoBids_Throws()
        {
            var ex = Assert.ThrowsAsync<RepositoryOperationException>(async () =>
            {
                await _service.GetAllBidAsync();
            });

            Assert.IsNotNull(ex.InnerException);
            Assert.IsInstanceOf<NotFoundException>(ex.InnerException);
        }

        [Test]
        public async Task DeleteBidAsync_Valid_DeletesSuccessfully()
        {
            var bid = new BidItem
            {
                Id = Guid.NewGuid(),
                AuctionItemId = Guid.NewGuid(),
                Amount = 100,
                BidderId = Guid.NewGuid(),
            };
            await _context.BidItems.AddAsync(bid);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteBidAsync(bid.Id);
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteBidAsync_BidNotFound_Throws()
        {
            Assert.ThrowsAsync<RepositoryOperationException>(() =>
                _service.DeleteBidAsync(Guid.NewGuid())
            );
        }
    }
}
