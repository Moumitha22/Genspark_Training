using Microsoft.AspNetCore.SignalR;
using Moq;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Misc;
using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Models.Enums;
using PropFinderApi.Services;

namespace PropFinderApi.Tests.Services
{
    [TestFixture]
    public class PropertyServiceTests
    {
        private Mock<IPropertyRepository> _propertyRepoMock;
        private Mock<IPropertyLocationRepository> _locationRepoMock;
        private Mock<IPropertyFeatureService> _featureServiceMock;
        private Mock<IListerProfileRepository> _listerProfileRepoMock;
        private Mock<IDiscountCodeRepository> _discountRepoMock;
        private Mock<IHubContext<NotificationHub>> _hubContextMock;
        private Mock<IHubClients> _hubClientsMock;
        private Mock<IClientProxy> _clientProxyMock;


        private PropertyService _propertyService;

        [SetUp]
        public void SetUp()
        {
            _propertyRepoMock = new Mock<IPropertyRepository>();
            _locationRepoMock = new Mock<IPropertyLocationRepository>();
            _featureServiceMock = new Mock<IPropertyFeatureService>();
            _listerProfileRepoMock = new Mock<IListerProfileRepository>();
            _discountRepoMock = new Mock<IDiscountCodeRepository>();
            _hubContextMock = new Mock<IHubContext<NotificationHub>>();
            _hubClientsMock = new Mock<IHubClients>();
            _clientProxyMock = new Mock<IClientProxy>();

            _hubContextMock.Setup(h => h.Clients).Returns(_hubClientsMock.Object);
            _hubClientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(_clientProxyMock.Object);

            _propertyService = new PropertyService(
                _propertyRepoMock.Object,
                _listerProfileRepoMock.Object,
                _locationRepoMock.Object,
                _discountRepoMock.Object,
                _featureServiceMock.Object,
                _hubContextMock.Object
            );
        }

        [Test]
        public async Task CreatePropertyAsync_WithValidFeaturesLocationDiscounts_ReturnsCreatedProperty()
        {
            var listerId = Guid.NewGuid();
            var discountId1 = Guid.NewGuid();
            var discountId2 = Guid.NewGuid();
            var featureId1 = Guid.NewGuid();
            var featureId2 = Guid.NewGuid();
            var optionId1 = Guid.NewGuid();

            var dto = new PropertyAddRequestDto
            {
                Title = "Luxury Villa",
                Description = "Sea-facing, newly renovated",
                Price = 1000000,
                ListerType = ListerType.Agent,
                PropertyType = PropertyType.House,
                ListingPurpose = ListingPurpose.Sale,
                AreaSqFt = 3000,
                Location = new PropertyLocationAddRequestDto
                {
                    Locality = "Juhu",
                    City = "Mumbai",
                    State = "Maharashtra",
                    Latitude = 19.0968m,
                    Longitude = 72.8260m
                },
                Features = new List<PropertyFeatureAddRequestDto>
                {
                    new PropertyFeatureAddRequestDto
                    {
                        FeatureId = featureId1,
                        Value = "Near Marine Drive",
                        DataType = "Text"
                    },
                    new PropertyFeatureAddRequestDto
                    {
                        FeatureId = featureId2,
                        OptionId = optionId1,
                        DataType = "Dropdown"
                    }
                },
                DiscountCodeIds = new List<Guid> { discountId1, discountId2 }
            };

            var profile = new ListerProfile
            {
                UserId = listerId,
                BusinessPhoneNumber = "8888888888"
            };

            var feature1 = new FeatureMaster
            {
                Id = featureId1,
                Name = "Nearby Landmark",
                DataType = FeatureDataType.Text,
                IsDeleted = false
            };

            var feature2 = new FeatureMaster
            {
                Id = featureId2,
                Name = "Furnishing",
                DataType = FeatureDataType.Dropdown,
                IsDeleted = false
            };

            var option1 = new FeatureOption
            {
                Id = optionId1,
                Value = "Fully Furnished"
            };

            var discount1 = new DiscountCode
            {
                Id = discountId1,
                Code = "WELCOME100",
                DiscountValue = 100,
                IsPercentage = false,
                FromDate = DateTime.Today,
                ToDate = DateTime.Today.AddDays(30),
                MaxListerLimit = 5,
                IsActive = true
            };

            var discount2 = new DiscountCode
            {
                Id = discountId2,
                Code = "SUMMER50",
                DiscountValue = 50,
                IsPercentage = true,
                FromDate = DateTime.Today,
                ToDate = DateTime.Today.AddDays(15),
                MaxListerLimit = 3,
                IsActive = true
            };


            _listerProfileRepoMock.Setup(r => r.GetByUserIdAsync(listerId)).ReturnsAsync(profile);
            _discountRepoMock.Setup(r => r.Get(discountId1)).ReturnsAsync(discount1);
            _discountRepoMock.Setup(r => r.Get(discountId2)).ReturnsAsync(discount2);
            _discountRepoMock.Setup(r => r.Update(It.IsAny<Guid>(), It.IsAny<DiscountCode>()))
                .ReturnsAsync((Guid id, DiscountCode code) => code);

            _propertyRepoMock.Setup(r => r.Add(It.IsAny<Property>()))
                .ReturnsAsync((Property p) =>
                {
                    p.Features = new List<PropertyFeature>
                        {
                            new PropertyFeature
                            {
                                FeatureId = featureId1,
                                Value = "Near Marine Drive",
                                Feature = feature1
                            },
                            new PropertyFeature
                            {
                                FeatureId = featureId2,
                                OptionId = optionId1,
                                Feature = feature2,
                                Option = option1
                            }
                        };

                    p.PropertyDiscountCodes = new List<PropertyDiscountCode>
                        {
                            new PropertyDiscountCode
                            {
                                DiscountCodeId = discountId1,
                                DiscountCode = discount1
                            },
                            new PropertyDiscountCode
                            {
                                DiscountCodeId = discountId2,
                                DiscountCode = discount2
                            }
                        };
                    p.PropertyImages = new List<PropertyImage>();
                    return p;
                });

            var result = await _propertyService.CreatePropertyAsync(dto, listerId);

            Assert.IsNotNull(result);
            Assert.That(result.Title, Is.EqualTo("Luxury Villa"));
            Assert.That(result.Location.City, Is.EqualTo("Mumbai"));
            Assert.That(result.FeatureSummary, Is.Not.Null.And.Not.Empty);
            Assert.That(result.FeatureSummary.Any(f => f.Values.Contains("Near Marine Drive")), Is.True);
            Assert.That(result.DiscountCodes, Is.Not.Null.And.Not.Empty);
            Assert.That(result.DiscountCodes.Any(d => d.Code == "WELCOME100"), Is.True);
            Assert.That(result.DiscountCodes.Any(d => d.DiscountValue == 50 && d.IsPercentage), Is.True);

            _propertyRepoMock.Verify(r => r.Add(It.IsAny<Property>()), Times.Once);
            _discountRepoMock.Verify(r => r.Update(discountId1, It.IsAny<DiscountCode>()), Times.Once);
            _discountRepoMock.Verify(r => r.Update(discountId2, It.IsAny<DiscountCode>()), Times.Once);
            _hubClientsMock.Verify(c => c.Group("Buyers"), Times.Once);
            _clientProxyMock.Verify(p =>
                p.SendCoreAsync("NewPropertyUploaded", It.IsAny<object[]>(), default), Times.Once);
        }

        [Test]
        public async Task CreatePropertyAsync_ValidRequest_AddsPropertyAndSendsNotification()
        {
            var listerId = Guid.NewGuid();
            var dto = new PropertyAddRequestDto
            {
                Title = "New Property",
                Price = 500000,
                ListerType = ListerType.Agent,
                PropertyType = PropertyType.Apartment,
                ListingPurpose = ListingPurpose.Sale,
                AreaSqFt = 1200,
                Location = new PropertyLocationAddRequestDto
                {
                    Locality = "Kothrud",
                    City = "Pune",
                    State = "Maharashtra"
                }
            };

            var profile = new ListerProfile
            {
                UserId = listerId,
                BusinessPhoneNumber = "1234567890"
            };

            _listerProfileRepoMock.Setup(r => r.GetByUserIdAsync(listerId)).ReturnsAsync(profile);
            _propertyRepoMock.Setup(r => r.Add(It.IsAny<Property>()))
                .ReturnsAsync((Property p) =>
                {
                    p.Features = new List<PropertyFeature>();
                    p.PropertyDiscountCodes = new List<PropertyDiscountCode>();
                    p.PropertyImages = new List<PropertyImage>();
                    return p;
                });

            var result = await _propertyService.CreatePropertyAsync(dto, listerId);

            Assert.IsNotNull(result);
            Assert.That(result.Title, Is.EqualTo("New Property"));

            _propertyRepoMock.Verify(r => r.Add(It.IsAny<Property>()), Times.Once);
            _hubClientsMock.Verify(c => c.Group("Buyers"), Times.Once);
            _clientProxyMock.Verify(p =>
                p.SendCoreAsync("NewPropertyUploaded", It.IsAny<object[]>(), default), Times.Once);
        }

        [Test]
        public void CreatePropertyAsync_MissingListerProfile_ThrowsNotFound()
        {
            var listerId = Guid.NewGuid();
            var dto = new PropertyAddRequestDto();

            _listerProfileRepoMock.Setup(r => r.GetByUserIdAsync(listerId)).ReturnsAsync((ListerProfile)null!);

            var ex = Assert.ThrowsAsync<NotFoundException>(() =>
                _propertyService.CreatePropertyAsync(dto, listerId));

            Assert.That(ex.Message, Does.Contain("complete your lister profile"));
        }

        [Test]
        public void CreatePropertyAsync_IncompleteProfile_ThrowsBadRequest()
        {
            var listerId = Guid.NewGuid();
            var profile = new ListerProfile { UserId = listerId, BusinessPhoneNumber = "" };
            var dto = new PropertyAddRequestDto();

            _listerProfileRepoMock.Setup(r => r.GetByUserIdAsync(listerId)).ReturnsAsync(profile);

            var ex = Assert.ThrowsAsync<BadRequestException>(() =>
                _propertyService.CreatePropertyAsync(dto, listerId));

            Assert.That(ex.Message, Does.Contain("Incomplete lister profile"));
        }

        [Test]
        public void CreatePropertyAsync_InvalidDiscountCode_ThrowsBadRequest()
        {
            var listerId = Guid.NewGuid();
            var dto = new PropertyAddRequestDto
            {
                Title = "Test Property",
                Price = 123000,
                AreaSqFt = 1000,
                ListerType = ListerType.Agent,
                PropertyType = PropertyType.Plot,
                ListingPurpose = ListingPurpose.Sale,
                Location = new PropertyLocationAddRequestDto
                {
                    Locality = "Baner",
                    City = "Pune",
                    State = "Maharashtra"
                },
                DiscountCodeIds = new List<Guid> { Guid.NewGuid() }
            };

            var profile = new ListerProfile
            {
                UserId = listerId,
                BusinessPhoneNumber = "9999999999"
            };
            _listerProfileRepoMock.Setup(r => r.GetByUserIdAsync(listerId)).ReturnsAsync(profile);
            _discountRepoMock.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((DiscountCode)null!);

            var ex = Assert.ThrowsAsync<BadRequestException>(() =>
                _propertyService.CreatePropertyAsync(dto, listerId));

            Assert.That(ex.Message, Does.Contain("Discount code"));
        }

        [Test]
        public void CreatePropertyAsync_DiscountCodeLimitReached_ThrowsBadRequest()
        {
            var listerId = Guid.NewGuid();
            var discountId = Guid.NewGuid();
            var dto = new PropertyAddRequestDto
            {
                Title = "Test Property",
                Price = 456000,
                AreaSqFt = 1500,
                ListerType = ListerType.Owner,
                PropertyType = PropertyType.House,
                ListingPurpose = ListingPurpose.Rent,
                Location = new PropertyLocationAddRequestDto
                {
                    Locality = "Andheri",
                    City = "Mumbai",
                    State = "Maharashtra"
                },
                DiscountCodeIds = new List<Guid> { discountId }
            };

            var profile = new ListerProfile
            {
                UserId = listerId,
                BusinessPhoneNumber = "2222222222"
            };
            var discount = new DiscountCode
            {
                Id = discountId,
                ListerUsageCount = 5,
                MaxListerLimit = 5
            };

            _listerProfileRepoMock.Setup(r => r.GetByUserIdAsync(listerId)).ReturnsAsync(profile);
            _discountRepoMock.Setup(r => r.Get(discountId)).ReturnsAsync(discount);

            var ex = Assert.ThrowsAsync<BadRequestException>(() =>
                _propertyService.CreatePropertyAsync(dto, listerId));

            Assert.That(ex.Message, Does.Contain("has reached its usage limit"));
        }

        [Test]
        public async Task GetAllPropertiesAsync_ReturnsAllProperties()
        {
            var property1 = new Property
            {
                Id = Guid.NewGuid(),
                Title = "Prop A",
                ListerId = Guid.NewGuid(),
                PropertyImages = new List<PropertyImage>(),
                PropertyDiscountCodes = new List<PropertyDiscountCode>(),
                Features = new List<PropertyFeature>(),
                Location = new PropertyLocation
                {
                    Id = Guid.NewGuid(),
                    City = "Test City",
                    State = "Test State"
                }
            };

            var property2 = new Property
            {
                Id = Guid.NewGuid(),
                Title = "Prop B",
                ListerId = Guid.NewGuid(),
                PropertyImages = new List<PropertyImage>(),
                PropertyDiscountCodes = new List<PropertyDiscountCode>(),
                Features = new List<PropertyFeature>(),
                Location = new PropertyLocation
                {
                    Id = Guid.NewGuid(),
                    City = "Another City",
                    State = "Another State"
                }
            };
            _propertyRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Property> { property1, property2 });

            var result = await _propertyService.GetAllPropertiesAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.Any(p => p.Title == "Prop A"), Is.True);
            Assert.That(result.Any(p => p.Title == "Prop B"), Is.True);
        }

        [Test]
        public async Task GetPropertyByIdAsync_ValidId_ReturnsProperty()
        {
            var propertyId = Guid.NewGuid();
            var property = new Property
            {
                Id = propertyId,
                Title = "Test Property",
                Location = new PropertyLocation
                {
                    Id = Guid.NewGuid(),
                    City = "City",
                    State = "State"
                },
                Features = new List<PropertyFeature>(),
                PropertyDiscountCodes = new List<PropertyDiscountCode>(),
                PropertyImages = new List<PropertyImage>()
            };

            _propertyRepoMock.Setup(r => r.Get(propertyId)).ReturnsAsync(property);

            var result = await _propertyService.GetPropertyByIdAsync(propertyId);

            Assert.IsNotNull(result);
            Assert.That(result.Title, Is.EqualTo("Test Property"));
            _propertyRepoMock.Verify(r => r.Get(propertyId), Times.Once);
        }

        [Test]
        public async Task GetPropertiesByListerIdAsync_ValidListerId_ReturnsPaginatedProperties()
        {
            var listerId = Guid.NewGuid();
            var pagination = new PaginationModel { Page = 1, PageSize = 10 };

            var property = new Property
            {
                Id = Guid.NewGuid(),
                Title = "Lister Property",
                ListerId = listerId,
                Location = new PropertyLocation(),
                Features = new List<PropertyFeature>(),
                PropertyDiscountCodes = new List<PropertyDiscountCode>(),
                PropertyImages = new List<PropertyImage>()
            };

            var paginatedResult = new PaginatedResult<Property>(
                new List<Property> { property },
                totalItems: 1,
                currentPage: 1,
                pageSize: 10
            );

            _propertyRepoMock
                .Setup(r => r.GetByListerIdAsync(listerId, pagination))
                .ReturnsAsync(paginatedResult);

            var result = await _propertyService.GetPropertiesByListerIdAsync(listerId, pagination);

            Assert.That(result.Items.Count, Is.EqualTo(1));
            Assert.That(result.Items.First().Title, Is.EqualTo("Lister Property"));
            _propertyRepoMock.Verify(r => r.GetByListerIdAsync(listerId, pagination), Times.Once);
        }


        [Test]
        public async Task BasicSearchPropertiesAsync_ValidSearch_ReturnsPaginatedResults()
        {
            var searchModel = new BasicPropertySearchModel
            {
                City = "Mumbai",
                ListingPurpose = ListingPurpose.Sale,
                PropertyTypes = new List<PropertyType> { PropertyType.Apartment }
            };
            var sortModel = new SortModel
            {
                SortBy = "price",
                Ascending = true,
            };
            var pagination = new PaginationModel { Page = 1, PageSize = 5 };

            var property = new Property
            {
                Id = Guid.NewGuid(),
                Title = "Basic Search Property",
                ListingPurpose = ListingPurpose.Sale,
                Location = new PropertyLocation { City = "Mumbai" },
                Features = new List<PropertyFeature>(),
                PropertyDiscountCodes = new List<PropertyDiscountCode>(),
                PropertyImages = new List<PropertyImage>()
            };

            var paginatedResult = new PaginatedResult<Property>(
                new List<Property> { property },
                totalItems: 1,
                currentPage: 1,
                pageSize: 5
            );

            _propertyRepoMock
                .Setup(r => r.BasicSearchAsync(searchModel, sortModel, pagination))
                .ReturnsAsync(paginatedResult);

            var result = await _propertyService.BasicSearchPropertiesAsync(searchModel, sortModel, pagination);

            Assert.That(result.Items.Count, Is.EqualTo(1));
            Assert.That(result.Items.First().Location.City, Is.EqualTo("Mumbai"));
            _propertyRepoMock.Verify(r => r.BasicSearchAsync(searchModel, sortModel, pagination), Times.Once);
        }

        [Test]
        public async Task AdvancedSearchPropertiesAsync_ValidSearch_ReturnsPaginatedResults()
        {
            var searchModel = new AdvancedPropertySearchModel
            {
                City = "Pune",
                ListingPurpose = ListingPurpose.Sale
            };
            var sortModel = new SortModel
            {
                SortBy = "areaSqFt",
                Ascending = false
            };
            var pagination = new PaginationModel { Page = 1, PageSize = 10 };

            var property = new Property
            {
                Id = Guid.NewGuid(),
                Title = "Advanced Property",
                Price = 800000,
                AreaSqFt = 2000,
                ListingPurpose = ListingPurpose.Sale,
                Location = new PropertyLocation { City = "Pune" },
                Features = new List<PropertyFeature>(),
                PropertyDiscountCodes = new List<PropertyDiscountCode>(),
                PropertyImages = new List<PropertyImage>()
            };

            var paginatedResult = new PaginatedResult<Property>(
                new List<Property> { property },
                totalItems: 1,
                currentPage: 1,
                pageSize: 10
            );

            _propertyRepoMock
                .Setup(r => r.AdvancedSearchAsync(searchModel, sortModel, pagination))
                .ReturnsAsync(paginatedResult);

            var result = await _propertyService.AdvancedSearchPropertiesAsync(searchModel, sortModel, pagination);

            Assert.That(result.Items.Count, Is.EqualTo(1));
            Assert.That(result.Items.First().Price, Is.EqualTo(800000));
            _propertyRepoMock.Verify(r => r.AdvancedSearchAsync(searchModel, sortModel, pagination), Times.Once);
        }

        [Test]
        public async Task AdvancedSearchPropertiesAsync_WithPriceAndAreaFilters_ReturnsFilteredResults()
        {
            var searchModel = new AdvancedPropertySearchModel
            {
                City = "Pune",
                PriceRange = new GenericRangeModel<decimal> { Min = 500000, Max = 900000 },
                AreaRange = new GenericRangeModel<decimal> { Min = 1000, Max = 2500 }
            };
            var sortModel = new SortModel { SortBy = "price", Ascending = true };
            var pagination = new PaginationModel { Page = 1, PageSize = 10 };

            var property = new Property
            {
                Id = Guid.NewGuid(),
                Title = "Range Property",
                Price = 700000,
                AreaSqFt = 1500,
                Location = new PropertyLocation { City = "Pune" },
                Features = new List<PropertyFeature>(),
                PropertyDiscountCodes = new List<PropertyDiscountCode>(),
                PropertyImages = new List<PropertyImage>()
            };

            var paginatedResult = new PaginatedResult<Property>(
                new List<Property> { property },
                totalItems: 1,
                currentPage: 1,
                pageSize: 10
            );

            _propertyRepoMock
                .Setup(r => r.AdvancedSearchAsync(searchModel, sortModel, pagination))
                .ReturnsAsync(paginatedResult);

            var result = await _propertyService.AdvancedSearchPropertiesAsync(searchModel, sortModel, pagination);

            Assert.That(result.Items.Count, Is.EqualTo(1));
            Assert.That(result.Items.First().Price, Is.EqualTo(700000));
        }

    }
}