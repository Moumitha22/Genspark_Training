using System;
using ClinicManagementSystem.Models.DTOs;

namespace ClinicManagementSystem.Interfaces
{
    public interface IOtherContextFunctionities
    {
        public Task<ICollection<DoctorsBySpecialityResponseDto>> GetDoctorsBySpeciality(string specilaity);
    }
}
