using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.Interfaces {

    public interface IPatientService
    {
        Task<Patient> AddPatient(Patient patient);
        Task<Patient> GetPatientByName(string name);

    }
}
