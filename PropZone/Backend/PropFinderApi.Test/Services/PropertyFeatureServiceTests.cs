using Moq;
using NUnit.Framework;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PropFinderApi.Tests.Services
{
    public class PropertyFeatureServiceTests
    {
        private Mock<IPropertyFeatureRepository> _propertyFeatureRepoMock;
        private PropertyFeatureService _service;

        [SetUp]
        public void Setup()
        {
            _propertyFeatureRepoMock = new Mock<IPropertyFeatureRepository>();
            _service = new PropertyFeatureService(_propertyFeatureRepoMock.Object);
        }

        [Test]
        public async Task UpdateFeatureSetAsync_ShouldAddNewFeatures()
        {
            var propertyId = Guid.NewGuid();
            var furnishingFeatureId = Guid.NewGuid();
            var selectedOptionId = Guid.NewGuid();

            _propertyFeatureRepoMock.Setup(r => r.GetByPropertyIdAsync(propertyId))
                .ReturnsAsync(new List<PropertyFeature>());

            var dtos = new List<PropertyFeatureAddRequestDto>
            {
                new()
                {
                    FeatureId = furnishingFeatureId,
                    DataType = "Dropdown",
                    OptionId = selectedOptionId
                }
            };

            List<PropertyFeature>? capturedFeatures = null;
            _propertyFeatureRepoMock
                .Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<PropertyFeature>>()))
                .Callback<IEnumerable<PropertyFeature>>(added => capturedFeatures = added.ToList());

            await _service.UpdateFeatureSetAsync(propertyId, dtos);

            Assert.That(capturedFeatures, Is.Not.Null);
            Assert.That(capturedFeatures.Count, Is.EqualTo(1));

            var addedFeature = capturedFeatures[0];
            Assert.That(addedFeature.FeatureId, Is.EqualTo(furnishingFeatureId));
            Assert.That(addedFeature.OptionId, Is.EqualTo(selectedOptionId));
            Assert.That(addedFeature.Value, Is.Null);
            Assert.That(addedFeature.PropertyId, Is.EqualTo(propertyId));

            _propertyFeatureRepoMock.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<PropertyFeature>>()), Times.Once);
            _propertyFeatureRepoMock.Verify(r => r.SaveAsync(), Times.Once);
        }


        [Test]
        public async Task UpdateFeatureSetAsync_ShouldUpdateExistingFeature()
        {
            var propertyId = Guid.NewGuid();
            var featureId = Guid.NewGuid();

            var existing = new PropertyFeature
            {
                Id = Guid.NewGuid(),
                PropertyId = propertyId,
                FeatureId = featureId,
                Value = "Old Value",
                IsDeleted = true,
            };

            _propertyFeatureRepoMock.Setup(r => r.GetByPropertyIdAsync(propertyId))
                .ReturnsAsync(new List<PropertyFeature> { existing });

            var dtos = new List<PropertyFeatureAddRequestDto>
            {
                new()
                {
                    FeatureId = featureId,
                    Value = "Updated Value",
                    DataType = "Text"
                }
            };

            await _service.UpdateFeatureSetAsync(propertyId, dtos);

            Assert.That(existing.Value, Is.EqualTo("Updated Value"));
            Assert.That(existing.IsDeleted, Is.False);
            _propertyFeatureRepoMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateFeatureSetAsync_ShouldSoftDeleteRemovedFeatures()
        {
            var propertyId = Guid.NewGuid();
            var existingFeature = new PropertyFeature
            {
                Id = Guid.NewGuid(),
                PropertyId = propertyId,
                FeatureId = Guid.NewGuid(),
                Value = "Old",
                IsDeleted = false
            };

            _propertyFeatureRepoMock.Setup(r => r.GetByPropertyIdAsync(propertyId))
                .ReturnsAsync(new List<PropertyFeature> { existingFeature });

            var dtos = new List<PropertyFeatureAddRequestDto>(); // Empty => all existing should be removed

            await _service.UpdateFeatureSetAsync(propertyId, dtos);

            Assert.That(existingFeature.IsDeleted, Is.True);
            _propertyFeatureRepoMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateFeatureSetAsync_ShouldHandleMixedAddUpdateDelete()
        {
            var propertyId = Guid.NewGuid();
            var keepFeatureId = Guid.NewGuid();
            var removeFeatureId = Guid.NewGuid();
            var addFeatureId = Guid.NewGuid();

            var existing = new List<PropertyFeature>
            {
                new() { PropertyId = propertyId, FeatureId = keepFeatureId, Value = "Old", IsDeleted = false },
                new() { PropertyId = propertyId, FeatureId = removeFeatureId, Value = "To be deleted", IsDeleted = false }
            };

            _propertyFeatureRepoMock.Setup(r => r.GetByPropertyIdAsync(propertyId))
                .ReturnsAsync(existing);

            var dtos = new List<PropertyFeatureAddRequestDto>
            {
                new() { FeatureId = keepFeatureId, Value = "Updated", DataType = "Text" },
                new() { FeatureId = addFeatureId, Value = "New", DataType = "Text" }
            };

            List<PropertyFeature>? added = null;
            _propertyFeatureRepoMock.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<PropertyFeature>>()))
                .Callback<IEnumerable<PropertyFeature>>(list => added = list.ToList());

            await _service.UpdateFeatureSetAsync(propertyId, dtos);

            Assert.That(added?.Count, Is.EqualTo(1));
            Assert.That(added[0].FeatureId, Is.EqualTo(addFeatureId));

            Assert.That(existing[0].Value, Is.EqualTo("Updated"));
            Assert.That(existing[1].IsDeleted, Is.True);

            _propertyFeatureRepoMock.Verify(r => r.SaveAsync(), Times.Once);
        }
    }
}

