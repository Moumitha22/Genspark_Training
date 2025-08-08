using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Exceptions;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Repositories;

namespace OnlineAuctionAPI.Tests
{
    public class RepositoryTests
    {
        private AuctionContext _context;
        private Repository<Guid, AuctionItem> _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AuctionContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new AuctionContext(options);
            _repository = new Repository<Guid, AuctionItem>(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Add_ValidItem_ReturnsItem()
        {
            var item = new AuctionItem { Id = Guid.NewGuid(), Name = "Test Item" };
            var result = await _repository.Add(item);
            Assert.AreEqual(item.Id, result.Id);
        }

        [Test]
        public void Add_NullItem_ThrowsRepositoryOperationException()
        {
            Assert.ThrowsAsync<RepositoryOperationException>(() => _repository.Add(null));
        }

        [Test]
        public async Task Get_ExistingItem_ReturnsItem()
        {
            var item = new AuctionItem { Id = Guid.NewGuid(), Name = "Test Item" };
            await _context.AuctionItems.AddAsync(item);
            await _context.SaveChangesAsync();

            var result = await _repository.Get(item.Id);
            Assert.IsNotNull(result);
        }

        [Test]
        public void Get_NonExistingItem_ThrowsRepositoryOperationException()
        {
            var key = Guid.NewGuid();
            var ex = Assert.ThrowsAsync<RepositoryOperationException>(() => _repository.Get(key));
            Assert.That(ex.InnerException, Is.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task GetAll_WithData_ReturnsList()
        {
            await _context.AuctionItems.AddAsync(
                new AuctionItem { Id = Guid.NewGuid(), Name = "Item 1" }
            );
            await _context.SaveChangesAsync();

            var result = await _repository.GetAll();
            Assert.IsNotEmpty(result);
        }

        [Test]
        public async Task GetAll_NoData_ReturnsEmptyList()
        {
            var result = await _repository.GetAll();
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task Update_ValidItem_UpdatesItem()
        {
            var item = new AuctionItem { Id = Guid.NewGuid(), Name = "Old Name" };
            await _context.AuctionItems.AddAsync(item);
            await _context.SaveChangesAsync();

            var updated = new AuctionItem { Id = item.Id, Name = "New Name" };
            var result = await _repository.Update(item.Id, updated);

            Assert.AreEqual("New Name", result.Name);
        }

        [Test]
        public void Update_NullItem_ThrowsRepositoryOperationException()
        {
            var id = Guid.NewGuid();
            Assert.ThrowsAsync<RepositoryOperationException>(() => _repository.Update(id, null));
        }

        [Test]
        public void Update_InvalidKey_ThrowsRepositoryOperationException()
        {
            var item = new AuctionItem { Id = Guid.NewGuid(), Name = "Update Fail" };
            var ex = Assert.ThrowsAsync<RepositoryOperationException>(() =>
                _repository.Update(item.Id, item)
            );
            Assert.IsNotNull(ex.InnerException);
            Assert.That(ex.InnerException, Is.TypeOf<RepositoryOperationException>());
        }

        [Test]
        public async Task Delete_ValidItem_DeletesItem()
        {
            var item = new AuctionItem { Id = Guid.NewGuid(), Name = "Delete Me" };
            await _context.AuctionItems.AddAsync(item);
            await _context.SaveChangesAsync();

            var result = await _repository.Delete(item.Id);
            Assert.AreEqual(item.Id, result.Id);
        }

        [Test]
        public void Delete_InvalidKey_ThrowsRepositoryOperationException()
        {
            var id = Guid.NewGuid();
            var ex = Assert.ThrowsAsync<RepositoryOperationException>(() => _repository.Delete(id));
            Assert.IsNotNull(ex.InnerException);
            Assert.That(ex.InnerException, Is.TypeOf<RepositoryOperationException>());
        }
    }
}
