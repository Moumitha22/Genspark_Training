using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.Interfaces
{

    public interface ISpecialityService
    {
        public Task<Speciality> GetSpecialityByName(string name);

    }
}
