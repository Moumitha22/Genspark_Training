using ClinicManagementSystem.Models;
using ClinicManagementSystem.Models.DTOs;

namespace ClinicManagementSystem.Interfaces {

    public interface IPatientService
    {
        public Task<Patient> AddPatient(PatientAddRequestDto patientAddRequestDto);
        public Task<ICollection<Patient>> GetAllPatients();
        public Task<Patient> GetPatientById(int id);
    }
}
