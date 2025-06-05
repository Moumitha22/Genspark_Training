using ClinicManagementSystem.Models;
using ClinicManagementSystem.Models.DTOs;

namespace ClinicManagementSystem.Misc
{
    public class PatientMapper
    {
        public Patient MapPatientAddRequestDtoToPatient(PatientAddRequestDto addRequestDto)
        {
            Patient Patient = new();
            Patient.Name = addRequestDto.Name;
            Patient.Email = addRequestDto.Email;
            Patient.Age = addRequestDto.Age;
            Patient.Phone = addRequestDto.Phone;

            return Patient;
        }
    }
}