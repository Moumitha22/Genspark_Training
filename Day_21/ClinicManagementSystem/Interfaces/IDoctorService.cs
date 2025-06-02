using ClinicManagementSystem.Models;
using ClinicManagementSystem.Models.DTOs;

namespace ClinicManagementSystem.Interfaces
{
    public interface IDoctorService
    {
        public Task<ICollection<DoctorResponseDto>> GetAllDoctors();
        public Task<DoctorResponseDto> GetDoctorById(int id);
        public Task<DoctorResponseDto> GetDoctorByName(string name);
        public Task<ICollection<DoctorsBySpecialityResponseDto>> GetDoctorsBySpeciality(string speciality);
        public Task<Doctor> AddDoctor(DoctorAddRequestDto doctor);
    }
}
