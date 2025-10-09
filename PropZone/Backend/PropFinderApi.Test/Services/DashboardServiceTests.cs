using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PropFinderApi.Contexts;
using PropFinderApi.Models;
using PropFinderApi.Models.Enums;
using PropFinderApi.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PropFinderApi.Tests.Services
{
    public class DashboardServiceTests
    {
        private PropFinderDbContext _context;
        private DashboardService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PropFinderDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            _context = new PropFinderDbContext(options);

            var user1 = new User { Id = Guid.NewGuid(), Email = "user1@test.com", Role = UserRole.Lister };
            var user2 = new User { Id = Guid.NewGuid(), Email = "user2@test.com", Role = UserRole.Lister };

            var property1 = new Property
            {
                Id = Guid.NewGuid(),
                ListerId = user1.Id,
                PropertyType = PropertyType.Apartment,
                ListingPurpose = ListingPurpose.Sale,
                Status = ListingStatus.Sold
            };

            var property2 = new Property
            {
                Id = Guid.NewGuid(),
                ListerId = user1.Id,
                PropertyType = PropertyType.House,
                ListingPurpose = ListingPurpose.Rent,
                Status = ListingStatus.Available
            };

            var property3 = new Property
            {
                Id = Guid.NewGuid(),
                ListerId = user2.Id,
                PropertyType = PropertyType.Apartment,
                ListingPurpose = ListingPurpose.Sale,
                Status = ListingStatus.Available
            };

            var inquiry1 = new ContactLog { Id = Guid.NewGuid(), PropertyId = property1.Id };
            var inquiry2 = new ContactLog { Id = Guid.NewGuid(), PropertyId = property2.Id };

            _context.Users.AddRange(user1, user2);
            _context.Properties.AddRange(property1, property2, property3);
            _context.ContactLogs.AddRange(inquiry1, inquiry2);
            _context.SaveChanges();

            _service = new DashboardService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAdminDashboardAsync_ReturnsCorrectCounts()
        {
            var result = await _service.GetAdminDashboardAsync();

            Assert.That(result.TotalUsers, Is.EqualTo(2));
            Assert.That(result.TotalProperties, Is.EqualTo(3));
            Assert.That(result.TotalInquiries, Is.EqualTo(2));
            Assert.That(result.TotalActiveListers, Is.EqualTo(2));
            Assert.That(result.PropertyTypeChart.Count, Is.GreaterThan(0));
            Assert.That(result.PropertyPurposeChart.Count, Is.GreaterThan(0));
            Assert.That(result.PropertyStatusChart.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task GetListerDashboardAsync_ReturnsCorrectStatsForLister()
        {
            var listerId = await _context.Properties.FirstAsync(p => p.PropertyType == PropertyType.Apartment).ContinueWith(t => t.Result.ListerId);
            var result = await _service.GetListerDashboardAsync(listerId);

            Assert.That(result.TotalPropertiesListed, Is.EqualTo(2));
            Assert.That(result.TotalForSale, Is.EqualTo(1));
            Assert.That(result.TotalForRent, Is.EqualTo(1));
            Assert.That(result.TotalSoldOut, Is.EqualTo(1));
            Assert.That(result.TotalRented, Is.EqualTo(0));
            Assert.That(result.TotalAvailable, Is.EqualTo(1));
            Assert.That(result.TotalInquiriesReceived, Is.EqualTo(2));
            Assert.That(result.PropertyTypeChart.Count, Is.GreaterThan(0));
        }
    }
}
