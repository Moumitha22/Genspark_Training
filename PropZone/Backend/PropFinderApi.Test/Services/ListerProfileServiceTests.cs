using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Mappers;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Services;

namespace PropFinderApi.Tests.Services
{
    [TestFixture]
    public class ListerProfileServiceTests
    {
        private Mock<IListerProfileRepository> _listerProfileRepoMock;
        private Mock<IRepository<Guid, User>> _userRepoMock;
        private ListerProfileService _service;

        [SetUp]
        public void Setup()
        {
            _listerProfileRepoMock = new Mock<IListerProfileRepository>();
            _userRepoMock = new Mock<IRepository<Guid, User>>();
            _service = new ListerProfileService(_listerProfileRepoMock.Object, _userRepoMock.Object);
        }

        [Test]
        public async Task CreateListerProfileAsync_Success()
        {
            var userId = Guid.NewGuid();
            var dto = new ListerProfileAddRequestDto
            {
                AgencyName = "Test Realty",
                BusinessPhoneNumber = "1234567890"
            };

            var user = new User { Id = userId };
            _userRepoMock.Setup(r => r.Get(userId)).ReturnsAsync(user);
            _listerProfileRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((ListerProfile?)null);

            var expectedProfile = new ListerProfile
            {
                UserId = userId,
                AgencyName = dto.AgencyName,
                BusinessPhoneNumber = dto.BusinessPhoneNumber
            };
            _listerProfileRepoMock.Setup(r => r.Add(It.IsAny<ListerProfile>())).ReturnsAsync(expectedProfile);

            var result = await _service.CreateListerProfileAsync(dto, userId);

            Assert.IsNotNull(result);
            Assert.That(result.AgencyName, Is.EqualTo(dto.AgencyName));
            Assert.That(result.BusinessPhoneNumber, Is.EqualTo(dto.BusinessPhoneNumber));
        }

        [Test]
        public void CreateListerProfileAsync_ProfileExists_ThrowsConflict()
        {
            var userId = Guid.NewGuid();
            var dto = new ListerProfileAddRequestDto
            {
                AgencyName = "Test Realty",
                BusinessPhoneNumber = "1234567890"
            };
            var user = new User { Id = userId, Email = "test@gmail.com" };
            var existingProfile = new ListerProfile { UserId = userId };

            _userRepoMock.Setup(r => r.Get(userId)).ReturnsAsync(user);
            _listerProfileRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(existingProfile);

            Assert.ThrowsAsync<ConflictException>(() =>
                _service.CreateListerProfileAsync(dto, userId));
        }

        [Test]
        public async Task GetAllAsync_ReturnsList()
        {
            var profiles = new List<ListerProfile>
            {
                new ListerProfile { Id = Guid.NewGuid(), AgencyName = "One" },
                new ListerProfile { Id = Guid.NewGuid(), AgencyName = "Two" }
            };

            _listerProfileRepoMock.Setup(r => r.GetAll()).ReturnsAsync(profiles);

            var result = await _service.GetAllAsync();

            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsProfile()
        {
            var id = Guid.NewGuid();
            var profile = new ListerProfile { Id = id };

            _listerProfileRepoMock.Setup(r => r.Get(id)).ReturnsAsync(profile);

            var result = await _service.GetByIdAsync(id);

            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(id));
        }

        [Test]
        public void GetByIdAsync_ProfileNotFound_ThrowsNotFoundException()
        {
            var id = Guid.NewGuid();

            _listerProfileRepoMock.Setup(r => r.Get(id)).ThrowsAsync(new NotFoundException("Profile not found"));

            Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await _service.GetByIdAsync(id);
            });
        }

        [Test]
        public async Task GetListerProfileByListerIdAsync_ReturnsProfile()
        {
            var listerId = Guid.NewGuid();
            var profile = new ListerProfile { UserId = listerId };

            _listerProfileRepoMock.Setup(r => r.GetByUserIdAsync(listerId)).ReturnsAsync(profile);

            var result = await _service.GetListerProfileByListerIdAsync(listerId);

            Assert.IsNotNull(result);
            Assert.That(result.UserId, Is.EqualTo(listerId));
        }

        [Test]
        public async Task GetListerProfileByListerIdAsync_ProfileNotFound_ReturnsNull()
        {
            var listerId = Guid.NewGuid();

            _listerProfileRepoMock.Setup(r => r.GetByUserIdAsync(listerId)).ReturnsAsync((ListerProfile?)null);

            var result = await _service.GetListerProfileByListerIdAsync(listerId);

            Assert.IsNull(result);
        }


        [Test]
        public async Task UpdateListerProfileAsync_Success()
        {
            var profileId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var dto = new ListerProfileAddRequestDto
            {
                AgencyName = "Updated Co",
                BusinessPhoneNumber = "0000000000"
            };

            var existing = new ListerProfile
            {
                Id = profileId,
                UserId = requesterId,
                AgencyName = "Old Co",
                BusinessPhoneNumber = "1111111111"
            };

            _listerProfileRepoMock.Setup(r => r.Get(profileId)).ReturnsAsync(existing);
            _listerProfileRepoMock
                .Setup(r => r.Update(profileId, It.IsAny<ListerProfile>()))
                .ReturnsAsync((Guid id, ListerProfile updated) => updated);

            var result = await _service.UpdateListerProfileAsync(profileId, dto, requesterId, "Lister");

            Assert.That(result.AgencyName, Is.EqualTo(dto.AgencyName));
            Assert.That(result.BusinessPhoneNumber, Is.EqualTo(dto.BusinessPhoneNumber));
        }

        [Test]
        public void UpdateListerProfileAsync_Unauthorized_Throws()
        {
            var profileId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var dto = new ListerProfileAddRequestDto
            {
                AgencyName = "Updated Co",
                BusinessPhoneNumber = "9876543210"
            };

            var profile = new ListerProfile { Id = profileId, UserId = Guid.NewGuid() };

            _listerProfileRepoMock.Setup(r => r.Get(profileId)).ReturnsAsync(profile);

            Assert.ThrowsAsync<UnauthorizedException>(() =>
                _service.UpdateListerProfileAsync(profileId, dto, requesterId, "Lister"));
        }


        [Test]
        public async Task IsProfileCompleteAsync_ProfileExistsWithPhoneNumber_ReturnsTrue()
        {
            var userId = Guid.NewGuid();
            var profile = new ListerProfile
            {
                UserId = userId,
                BusinessPhoneNumber = "1234567890"
            };

            _listerProfileRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(profile);

            var result = await _service.IsProfileCompleteAsync(userId);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task IsProfileCompleteAsync_ProfileDoesNotExist_ReturnsFalse()
        {
            var userId = Guid.NewGuid();

            _listerProfileRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync((ListerProfile?)null);

            var result = await _service.IsProfileCompleteAsync(userId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task IsProfileCompleteAsync_ProfileExistsWithoutPhoneNumber_ReturnsFalse()
        {
            var userId = Guid.NewGuid();
            var profile = new ListerProfile
            {
                UserId = userId,
                BusinessPhoneNumber = ""
            };

            _listerProfileRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(profile);

            var result = await _service.IsProfileCompleteAsync(userId);

            Assert.IsFalse(result);
        }

    }

}
