using ClinicManagementSystem.Contexts;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Repositories;
using ClinicManagementSystem.Interfaces;
using ClinicManagementSystem.Services;
using ClinicManagementSystem.Models.DTOs;
using ClinicManagementSystem.Misc;
using Microsoft.EntityFrameworkCore;
using Moq;
using AutoMapper;
using NUnit.Framework;

namespace ClinicManagementSystem.Test;

[TestFixture]
public class DoctorServiceTest
{
    private ClinicContext _context;
    private Mock<DoctorRepository> doctorRepositoryMock;
    private Mock<SpecialityRepository> specialityRepositoryMock;
    private Mock<DoctorSpecialityRepository> doctorSpecialityRepositoryMock;
    private Mock<UserRepository> userRepositoryMock;
    private Mock<OtherFunctionalitiesImplementation> otherContextFunctionalitiesMock;
    private Mock<EncryptionService> encryptionServiceMock;
    private Mock<IMapper> mapperMock;

    private IDoctorService doctorService;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ClinicContext>()
                            .UseInMemoryDatabase("TestDb")
                            .Options;
        _context = new ClinicContext(options);

        doctorRepositoryMock = new Mock<DoctorRepository>(_context);
        specialityRepositoryMock = new Mock<SpecialityRepository>(_context);
        doctorSpecialityRepositoryMock = new Mock<DoctorSpecialityRepository>(_context);
        userRepositoryMock = new Mock<UserRepository>(_context);
        otherContextFunctionalitiesMock = new Mock<OtherFunctionalitiesImplementation>(_context);
        encryptionServiceMock = new Mock<EncryptionService>();
        mapperMock = new Mock<IMapper>();

        doctorService = new DoctorService(
            doctorRepositoryMock.Object,
            specialityRepositoryMock.Object,
            doctorSpecialityRepositoryMock.Object,
            userRepositoryMock.Object,
            otherContextFunctionalitiesMock.Object,
            encryptionServiceMock.Object,
            mapperMock.Object
        );
    }

    [TestCase("General")]
    public async Task TestGetDoctorBySpeciality(string speciality)
    {
        // Arrange
        otherContextFunctionalitiesMock.Setup(ocf => ocf.GetDoctorsBySpeciality(It.IsAny<string>()))
            .ReturnsAsync(new List<DoctorsBySpecialityResponseDto>
            {
                new DoctorsBySpecialityResponseDto {
                    DoctorName = "test",
                    Yoe = 2,
                    Id = 1 }
            });

        // Act
        var result = await doctorService.GetDoctorsBySpeciality(speciality);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
    }

    // [Test]
    // public async Task AddDoctor_ShouldAddDoctorAndUser_WhenValidInput()
    // {
    //     // Arrange
    //     var doctorDto = new DoctorAddRequestDto
    //     {
    //         Name = "doc@example.com",
    //         Password = "1234",
    //         Specialities = new List<SpecialityAddRequestDto>
    //         {
    //             new SpecialityAddRequestDto { Name = "Cardiology" }
    //         }
    //     };

    //     var encryptedModel = new EncryptModel
    //     {
    //         EncryptedData = new byte[] { 1, 2, 3 },
    //         HashKey = new byte[] { 4, 5, 6 }
    //     };

    //     var user = new User { Username = "doc@example.com", Role = "Doctor" };
    //     var doctor = new Doctor { Id = 1, Name = "Dr. Smith" };
    //     var speciality = new Speciality { Id = 1, Name = "Cardiology" };
    //     var doctorSpeciality = new DoctorSpeciality { DoctorId = 1, SpecialityId = 1 };

    //     mapperMock.Setup(m => m.Map<DoctorAddRequestDto, User>(doctorDto)).Returns(user);
    //     encryptionServiceMock.Setup(e => e.EncryptData(It.IsAny<EncryptModel>())).ReturnsAsync(encryptedModel);
    //     userRepositoryMock.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync(user);
    //     doctorRepositoryMock.Setup(r => r.Add(It.IsAny<Doctor>())).ReturnsAsync(doctor);
    //     specialityRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Speciality>());
    //     specialityRepositoryMock.Setup(r => r.Add(It.IsAny<Speciality>())).ReturnsAsync(speciality);
    //     doctorSpecialityRepositoryMock.Setup(r => r.Add(It.IsAny<DoctorSpeciality>())).ReturnsAsync(doctorSpeciality);

    //     // Act
    //     var result = await doctorService.AddDoctor(doctorDto);

    //     // Assert
    //     Assert.NotNull(result);
    //     Assert.That(result.Id, Is.EqualTo(1));
    //     userRepositoryMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
    //     doctorRepositoryMock.Verify(r => r.Add(It.IsAny<Doctor>()), Times.Once);
    //     specialityRepositoryMock.Verify(r => r.Add(It.IsAny<Speciality>()), Times.Once);
    //     doctorSpecialityRepositoryMock.Verify(r => r.Add(It.IsAny<DoctorSpeciality>()), Times.Once);
    // }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    
}
