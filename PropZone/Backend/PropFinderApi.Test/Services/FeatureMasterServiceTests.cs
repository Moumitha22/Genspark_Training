using Moq;
using NUnit.Framework;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Models.Enums;
using PropFinderApi.Services;

namespace PropFinderApi.Tests.Services
{
    public class FeatureMasterServiceTests
    {
        private Mock<IFeatureMasterRepository> _featureRepoMock;
        private FeatureMasterService _service;

        [SetUp]
        public void Setup()
        {
            _featureRepoMock = new Mock<IFeatureMasterRepository>();
            _service = new FeatureMasterService(_featureRepoMock.Object);
        }

        [Test]
        public async Task CreateFeatureAsync_ShouldCreateFeatureWithOptionsAndApplicability()
        {
            var dto = new FeatureMasterAddRequestDto
            {
                Name = "Furnishing",
                DataType = FeatureDataType.Dropdown,
                FilterMode = FeatureFilterMode.Exact,
                Options = new List<string> { "FullyFurnished", "Semifurnished", "Unfurnished" },
                Applicabilities = new List<FeatureApplicabilityDto>
                {
                    new() { AppliesToType = PropertyType.Apartment, AppliesToPurpose = ListingPurpose.Sale }
                }
            };

            var result = await _service.CreateFeatureAsync(dto);

            Assert.AreEqual(dto.Name, result.Name);
            Assert.AreEqual("Dropdown", result.DataType);
            Assert.AreEqual(3, result.Options.Count);
            _featureRepoMock.Verify(r => r.Add(It.IsAny<FeatureMaster>()), Times.Once);
        }

        [Test]
        public async Task GetAsync_ShouldReturnFeature()
        {
            var featureId = Guid.NewGuid();
            var feature = new FeatureMaster { Id = featureId, Name = "Test" };
            _featureRepoMock.Setup(r => r.Get(featureId)).ReturnsAsync(feature);

            var result = await _service.GetAsync(featureId);

            Assert.AreEqual(featureId, result.Id);
        }

        [Test]
        public async Task GetAllFeaturesAsync_ShouldReturnAllFeaturesMapped()
        {
            var features = new List<FeatureMaster>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test",
                    DataType = FeatureDataType.Boolean,
                    FilterMode = FeatureFilterMode.Boolean,
                    Options = new List<FeatureOption> { new() { Id = Guid.NewGuid(), Value = "Yes", IsDeleted = false } },
                    Applicability = new List<FeatureApplicability> { new() { AppliesToPurpose = ListingPurpose.Sale, AppliesToType = PropertyType.Apartment, IsDeleted = false } }
                }
            };
            _featureRepoMock.Setup(r => r.GetAll()).ReturnsAsync(features);

            var result = await _service.GetAllFeaturesAsync();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Test", result.First().Name);
        }

        [Test]
        public async Task GetApplicableFeaturesAsync_ShouldReturnMappedFeatures()
        {
            var features = new List<FeatureMaster>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "HasBalcony",
                    DataType = FeatureDataType.Boolean,
                    FilterMode = FeatureFilterMode.Boolean,
                    Options = new List<FeatureOption> { new() { Id = Guid.NewGuid(), Value = "Yes", IsDeleted = false } }
                }
            };
            _featureRepoMock.Setup(r => r.GetApplicableFeaturesAsync(PropertyType.Apartment, ListingPurpose.Sale))
                            .ReturnsAsync(features);

            var result = await _service.GetApplicableFeaturesAsync(PropertyType.Apartment, ListingPurpose.Sale);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("HasBalcony", result.First().Name);
        }

        [Test]
        public async Task GetApplicableFeaturesByPurposeAsync_ShouldReturnMappedFeatures()
        {
            var features = new List<FeatureMaster>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Facing",
                    DataType = FeatureDataType.Dropdown,
                    FilterMode = FeatureFilterMode.Exact,
                    Options = new List<FeatureOption> { new() { Id = Guid.NewGuid(), Value = "East", IsDeleted = false } }
                }
            };
            _featureRepoMock.Setup(r => r.GetApplicableFeaturesByPurposeAsync(ListingPurpose.Rent))
                            .ReturnsAsync(features);

            var result = await _service.GetApplicableFeaturesByPurposeAsync(ListingPurpose.Rent);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Facing", result.First().Name);
        }

        [Test]
        public async Task SoftDeleteFeatureAsync_ShouldMarkFeatureAsDeleted()
        {
            var featureId = Guid.NewGuid();
            var feature = new FeatureMaster
            {
                Id = featureId,
                Name = "Lift",
                IsDeleted = false
            };

            _featureRepoMock.Setup(r => r.Get(featureId)).ReturnsAsync(feature);
            _featureRepoMock.Setup(r => r.Update(featureId, It.IsAny<FeatureMaster>()))
                .Callback<Guid, FeatureMaster>((id, updatedFeature) =>
                {
                    feature.IsDeleted = updatedFeature.IsDeleted;
                });

            var result = await _service.SoftDeleteFeatureAsync(featureId);

            Assert.IsTrue(result);
            Assert.IsTrue(feature.IsDeleted);

            _featureRepoMock.Verify(r => r.Update(featureId, It.Is<FeatureMaster>(f =>
                f.IsDeleted 
            )), Times.Once);
        }


        [Test]
        public void SoftDeleteFeatureAsync_ShouldThrowIfAlreadyDeleted()
        {
            var featureId = Guid.NewGuid();
            var feature = new FeatureMaster { Id = featureId, Name = "Lift", IsDeleted = true };
            _featureRepoMock.Setup(r => r.Get(featureId)).ReturnsAsync(feature);

            Assert.ThrowsAsync<ConflictException>(() => _service.SoftDeleteFeatureAsync(featureId));
        }


        [Test]
        public async Task UpdateFeatureAsync_ShouldUpdateAndReturnUpdatedFeature()
        {
            var featureId = Guid.NewGuid();

            var existing = new FeatureMaster
            {
                Id = featureId,
                Name = "Old Name",
                DataType = FeatureDataType.Dropdown,
                FilterMode = FeatureFilterMode.Exact,
                Options = new List<FeatureOption>
                    {
                        new() { Id = Guid.NewGuid(), Value = "Old", IsDeleted = false }
                    },
                IsDeleted = false
            };

            var dto = new FeatureMasterAddRequestDto
            {
                Name = "Updated",
                DataType = FeatureDataType.MultiSelect,
                FilterMode = FeatureFilterMode.Exact,
                Options = new List<string> { "New" },
                Applicabilities = new List<FeatureApplicabilityDto>()
            };

            var updated = new FeatureMaster
            {
                Id = featureId,
                Name = "Updated",
                DataType = FeatureDataType.MultiSelect,
                FilterMode = FeatureFilterMode.Exact,
                Options = new List<FeatureOption>
                    {
                        new() { Id = Guid.NewGuid(), Value = "New", IsDeleted = false }
                    },
                IsDeleted = false
            };

            _featureRepoMock.Setup(r => r.Get(featureId)).ReturnsAsync(existing);
            _featureRepoMock.Setup(r => r.UpdateFeatureWithOptionsAsync(featureId, dto)).Returns(Task.CompletedTask);
            _featureRepoMock.Setup(r => r.Get(featureId)).ReturnsAsync(updated);

            var result = await _service.UpdateFeatureAsync(featureId, dto);

            Assert.AreEqual("Updated", result.Name);
            Assert.AreEqual("MultiSelect", result.DataType);
            Assert.AreEqual(1, result.Options.Count);
        }


        [Test]
        public void UpdateFeatureAsync_ShouldThrowIfDtoIsNull()
        { 
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateFeatureAsync(Guid.NewGuid(), null));
        }
    }
}
